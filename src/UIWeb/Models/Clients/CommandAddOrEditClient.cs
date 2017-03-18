using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Files;
using ImageResizer;
using MediatR;
using NExtensions;
using File = Wiz.Gringotts.UIWeb.Models.Files.File;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class AddOrEditClientCommand : IAsyncRequest<ICommandResult>
    {
        public ModelStateDictionary ModelState { get; private set; }
        public ClientEditorForm Editor { get; private set; }

        public AddOrEditClientCommand(ClientEditorForm editor, ModelStateDictionary modelState)
        {
            this.ModelState = modelState;
            this.Editor = editor;
        }
    }

    public class ClientEditorFormIdentifierValidatorHandler : CommandValidator<AddOrEditClientCommand>
    {
       
        private readonly ApplicationDbContext dbContext;


        public ClientEditorFormIdentifierValidatorHandler(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Validators = new Action<AddOrEditClientCommand>[]
            {
                EnsureIdentifierTypesAreDistinct, EnsureSSNIdentifiersAreValid, EnsureIdentifiersAreNotInUse
            };
        }

        public void EnsureIdentifierTypesAreDistinct(AddOrEditClientCommand request)
        {
            Logger.Trace("EnsureIdentifierTypesAreDistinct");

            var duplicatesTypeIds = (from item in request.Editor.Identifiers
                group item by item.TypeId into grp
                where grp.Count() > 1
                select grp.Key)
                .ToArray();

            var seenTypeIds = new List<int>();

            if (!duplicatesTypeIds.Any()) 
                return;
            
            for (var i = 0; i < request.Editor.Identifiers.Count; i++)
            {
                var identifier = request.Editor.Identifiers[i];

                if (duplicatesTypeIds.Any(d => d == identifier.TypeId) && seenTypeIds.Contains(identifier.TypeId))
                    request.ModelState.AddModelError("Identifiers[{0}].TypeId".FormatWith(i), "Duplicate System");
                
                seenTypeIds.Add(identifier.TypeId);
            }
        }

        public void EnsureSSNIdentifiersAreValid(AddOrEditClientCommand request)
        {
            Logger.Trace("EnsureSSNIdentifiersAreValid");

            var ssnType = request.Editor.IdentifierTypes
                .Single(t => t.Name.Equals("SSN", StringComparison.InvariantCultureIgnoreCase));
            var identifiers = request.Editor.Identifiers;
            var invalid = (from item in identifiers
                where item.TypeId == ssnType.Id
                where !Regex.IsMatch(item.Value, @"^\d{3}-\d{2}-\d{4}$")
                select item)
                .ToArray();
            
            if (!invalid.Any())
                return;

            for (var i = 0; i < request.Editor.Identifiers.Count; i++)
            {
                var identifier = request.Editor.Identifiers[i];
                if(invalid.Any(d => d.TypeId == identifier.TypeId && d.Value == identifier.Value))
                    request.ModelState.AddModelError("Identifiers[{0}].Value".FormatWith(i), "Invalid SSN");
            }
        }


        public void EnsureIdentifiersAreNotInUse(AddOrEditClientCommand request)
        {
            Logger.Trace("EnsureIdentifiersAreNotInUse");

            var identifiers = request.Editor.Identifiers;
            var keys = identifiers.Select(i => i.TypeId + "~" + i.Value).ToArray();

            var inuse = from i in dbContext.ClientIdentifiers
                where i.Client.Id != request.Editor.ClientId
                where keys.Contains(i.ClientIdentifierType.Id + "~" + i.Value)
                select i;

            if (!inuse.Any())
                return;

            for (var i = 0; i < identifiers.Count; i++)
            {
                var identifier = identifiers[i];
                var exists = inuse.Any(e => e.ClientIdentifierType.Id == identifier.TypeId
                                            && e.Value.Equals(identifier.Value));

                if (exists)
                    request.ModelState.AddModelError("Identifiers[{0}].Value".FormatWith(i),
                        "System Id ({0}) is already in use".FormatWith(identifier.Value));
            }
        }

    }

    public class ClientEditorFormPayeesValidatorHandler : CommandValidator<AddOrEditClientCommand>
    {

        private readonly ApplicationDbContext dbContext;

        public ClientEditorFormPayeesValidatorHandler(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Validators = new Action<AddOrEditClientCommand>[]
            {
                EnsureAttorneysAreUnique, EnsureGuardiansAreUnique
            };
        }

        public void EnsureAttorneysAreUnique(AddOrEditClientCommand request)
        {
            Logger.Trace("EnsureAttorneysAreUnique");

            var duplciateIds = (from item in request.Editor.Attorneys
                group item by item.Id into grp
                where grp.Count() > 1
                select grp.Key)
                .ToArray();

            var seenIds = new List<int>();

            if (!duplciateIds.Any())
                return;

            for (var i = 0; i < request.Editor.Attorneys.Count; i++)
            {
                var attorney = request.Editor.Attorneys[i];

                if (duplciateIds.Any(d => d == attorney.Id) && seenIds.Contains(attorney.Id))
                    request.ModelState.AddModelError<ClientEditorForm>(m => m.Attorneys[i].Name , "Duplicate Attorney");

                seenIds.Add(attorney.Id);
            }
        }

        public void EnsureGuardiansAreUnique(AddOrEditClientCommand request)
        {
            Logger.Trace("EnsureGuardiansAreUnique");

            var duplciateIds = (from item in request.Editor.Guardians
                group item by item.Id into grp
                where grp.Count() > 1
                select grp.Key)
                .ToArray();

            var seenIds = new List<int>();

            if (!duplciateIds.Any())
                return;

            for (var i = 0; i < request.Editor.Guardians.Count; i++)
            {
                var guardian = request.Editor.Guardians[i];

                if (duplciateIds.Any(d => d == guardian.Id) && seenIds.Contains(guardian.Id))
                    request.ModelState.AddModelError<ClientEditorForm>(m => m.Guardians[i].Name, "Duplicate Guardian");

                seenIds.Add(guardian.Id);
            }
        }
    }

    public class ClientEditorFormImageHandler : IAsyncPreRequestHandler<AddOrEditClientCommand>
    {
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public ClientEditorFormImageHandler(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task Handle(AddOrEditClientCommand request)
        {
            Logger.Trace("Handle");

            if (request.Editor.Image != null && request.Editor.Image.ContentLength > 0)
            {
                request.Editor.ImageId = await StoreImage(request.Editor);
            }

        }

        private async Task<int?> StoreImage(ClientEditorForm editor)
        {
            var settings = new ResizeSettings
            {
                Width = 300,
                Height = 300,
                CropTopLeft = new PointF(editor.ImageX, editor.ImageY),
                CropBottomRight = new PointF(editor.ImageX + editor.ImageWidth, editor.ImageY + editor.ImageHeight)
            };

            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(editor.Image, ms, settings);

                var file = new File
                {
                    FileName = Path.GetFileName(editor.Image.FileName),
                    FileType = FileType.Image,
                    ContentType = editor.Image.ContentType,
                    Content = ms.ToByteArray()
                };

                context.Files.Add(file);
                await context.SaveChangesAsync();

                return file.Id;
            }
            
        }
    }

    public class AddOrEditClientCommandHandler : IAsyncRequestHandler<AddOrEditClientCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public AddOrEditClientCommandHandler(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public async Task<ICommandResult> Handle(AddOrEditClientCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.ClientId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditClientCommand message)
        {
            Logger.Trace("Add");

            var organization = tenantOrganizationProvider.GetTenantOrganization();
            var client = message.Editor.BuildClient(context);
            var residency = client.AddResidency(organization);

            message.Editor.UpdateResidency(residency, context);

            context.Clients.Add(client);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", client.Id);

            return new SuccessResult(client.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditClientCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.ClientId);

            var organization = tenantOrganizationProvider.GetTenantOrganization();
            var client = context.Clients
                .Include(c=>c.Identifiers)
                .Single(c => c.Id == message.Editor.ClientId);

            var residency = client.Residencies.First(r => r.OrganizationId == organization.Id);
            
            message.Editor.UpdateClient(client, context);

            message.Editor.UpdateResidency(residency, context);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", client.Id);

            return new SuccessResult(client.Id);
        }
    }

    public class AddOrUpdateClientPostHandler : IAsyncPostRequestHandler<AddOrEditClientCommand, ICommandResult>
    {
        private readonly ApplicationDbContext context;
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public AddOrUpdateClientPostHandler(ApplicationDbContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task Handle(AddOrEditClientCommand command, ICommandResult result)
        {
            Logger.Trace("Handle");

            if (result.IsFailure)
                return;

            var client = await context.Clients
                .FirstAsync(c => c.Id == (int) result.Result);

            await mediator.PublishAsync(new ClientAddedOrUpdatedNotification(command, client));
        }
    }
}