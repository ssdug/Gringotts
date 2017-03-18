using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class AddOrEditReceiptSourceCommand : IAsyncRequest<ICommandResult>
    {
        public ReceiptSourceEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public AddOrEditReceiptSourceCommand(ReceiptSourceEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class ReceiptSourceEditorFormValidatorHandler : CommandValidator<AddOrEditReceiptSourceCommand>
    {
        private readonly ISearch<ReceiptSource> sources;

        public ReceiptSourceEditorFormValidatorHandler(ISearch<ReceiptSource> sources)
        {
            this.sources = sources;
            Validators = new Action<AddOrEditReceiptSourceCommand>[]
            {
                EnsureReceiptSourceIsDistinct
            };
        }

        private void EnsureReceiptSourceIsDistinct(AddOrEditReceiptSourceCommand request)
        {
            Logger.Trace("EnsureReceiptSourceIsDistinct");

            var duplicateSources = sources.All()
                .Where(c => c.Id != request.Editor.ReceiptSourceId)
                .Any(c => c.Name == request.Editor.Name);

            if (!duplicateSources)
                return;

            request.ModelState.AddModelError("Name", "Duplicate Receipt Source not allowed");
        }
    }

    public class AddOrEditReceiptSourceCommandHandler : IAsyncRequestHandler<AddOrEditReceiptSourceCommand, ICommandResult>
    {
        private readonly ISearch<ReceiptSource> sources;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditReceiptSourceCommandHandler(ISearch<ReceiptSource> sources, ApplicationDbContext context)
        {
            this.sources = sources;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditReceiptSourceCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.ReceiptSourceId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditReceiptSourceCommand message)
        {
            Logger.Trace("Add");

            var source = message.Editor.BuildReceiptSource(context);

            context.ReceiptSources.Add(source);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", source.Id);

            return new SuccessResult(source.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditReceiptSourceCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.ReceiptSourceId);

            var source = await sources.GetById(message.Editor.ReceiptSourceId.Value)
                .SingleAsync();

            message.Editor.UpdateReceiptSource(source, context);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", source.Id);

            return new SuccessResult(source.Id);
        }
    }
}