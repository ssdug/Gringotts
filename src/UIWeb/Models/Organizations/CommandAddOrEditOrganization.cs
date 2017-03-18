using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Features;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class AddOrEditOrganizationCommand : IAsyncRequest<ICommandResult>
    {
        public OrganizationEditorForm Editor { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        public AddOrEditOrganizationCommand(OrganizationEditorForm editor, ModelStateDictionary modelState)
        {
            Editor = editor;
            ModelState = modelState;
        }
    }

    public class OrganizationEditorFormValidatorHandler : CommandValidator<AddOrEditOrganizationCommand>
    {
        private readonly ISearch<Organization> organizations;

        public OrganizationEditorFormValidatorHandler(ISearch<Organization> organizations)
        {
            this.organizations = organizations;
            Validators = new Action<AddOrEditOrganizationCommand>[]
            {
                EnsureNameIsUnique, EnsureGroupIsUnique, EnsureAbbreviationIsUnique
            };
        }

        public void EnsureNameIsUnique(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("EnsureNameIsUnique::{0}", message.Editor.Name);

            var isUnique = !organizations.All()
                .Where(o => o.Id != message.Editor.OrganizationId)
                .Any(o => o.Name.Equals(message.Editor.Name));

            if(isUnique)
                return;

            message.ModelState.AddModelError("Name", "The organization name ({0}) is in use by another organization."
                    .FormatWith(message.Editor.Name));
        }

        public void EnsureGroupIsUnique(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("EnsureGroupIsUnique::{0}", message.Editor.GroupName);

            var isUnique = !organizations.All()
                .Where(o => o.Id != message.Editor.OrganizationId)
                .Any(o => o.GroupName.Equals(message.Editor.GroupName));

            if (isUnique)
                return;

            message.ModelState.AddModelError("Group", "The Active Directory Group ({0}) is in use by another organization."
                    .FormatWith(message.Editor.GroupName));
        }

        public void EnsureAbbreviationIsUnique(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("EnsureAbbreviationIsUnique::{0}", message.Editor.Abbreviation);

            var isUnique = !organizations.All()
                .Where(o => o.Id != message.Editor.OrganizationId)
                .Any(o => o.Abbreviation.Equals(message.Editor.Abbreviation, StringComparison.InvariantCultureIgnoreCase));

            if (isUnique)
                return;

            message.ModelState.AddModelError("Abbreviation", "The Abbreviation ({0}) is in use by another organization."
                        .FormatWith(message.Editor.Name));
        }
    }

    public class AddOrEditOrganizationCommandHandler : IAsyncRequestHandler<AddOrEditOrganizationCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Organization> organizations;
        private readonly ApplicationDbContext context;

        public AddOrEditOrganizationCommandHandler(ISearch<Organization> organizations, ApplicationDbContext context)
        {
            this.organizations = organizations;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.OrganizationId.HasValue)
                return await Edit(message);
            
            return await Add(message);
        }


        private async Task<ICommandResult> Add(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("Add");

            var organization = message.Editor.BuildOrganiation(context);
            context.Organizations.Add(organization);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", organization.Id);

            return new SuccessResult(organization.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditOrganizationCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.OrganizationId);

            var organization = await organizations.GetById(message.Editor.OrganizationId.Value)
                .FirstAsync();

            message.Editor.UpdateOrganization(organization);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", organization.Id);

            return new SuccessResult(organization.Id);
        }
    }

    public class UpdateOrganizationFeaturesPostHandler : IAsyncPostRequestHandler<AddOrEditOrganizationCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Organization> organizations;
        private readonly ApplicationDbContext context;
        private readonly IMediator mediator;

        public UpdateOrganizationFeaturesPostHandler(ISearch<Organization> organizations, ApplicationDbContext context, IMediator mediator)
        {
            this.organizations = organizations;
            this.context = context;
            this.mediator = mediator;
        }

        public async Task Handle(AddOrEditOrganizationCommand command, ICommandResult result)
        {
            Logger.Trace("Handle");

            if (result.IsFailure)
                return;
            
            var organization = await organizations.GetById((int) result.Result)
                                        .Include(o => o.Features)
                                        .FirstAsync();
            
            var enabledFeatures = command.Editor.EnabledFeatures
                                    .Where(i => i.All(char.IsDigit))
                                    .Select(int.Parse).ToHashSet();

            //disable features
            var disabled = organization.Features
                            .Where(f => !enabledFeatures.Contains(f.Id))
                            .ToHashSet();

            //enable features    
            var enabled = context.Features
                            .Where(f => enabledFeatures.Contains(f.Id))
                            .ToHashSet();
            
            disabled.ForEach(feature => organization.Features.Remove(feature));

            enabled.ForEach(feature => organization.Features.Add(feature));
            
            await context.SaveChangesAsync();

            await Task.WhenAll(disabled.Select(feature => 
                mediator.PublishAsync(new FeatureDisabledNotification(organization, feature))));

            await Task.WhenAll(enabled.Select(feature => 
                mediator.PublishAsync(new FeatureEnabledNotification(organization, feature))));
        }
    }
}