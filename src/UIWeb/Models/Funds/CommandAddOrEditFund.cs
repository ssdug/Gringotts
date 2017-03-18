using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class AddOrEditFundCommand : IAsyncRequest<ICommandResult>
    {
        public FundEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public AddOrEditFundCommand(FundEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class AddOrEditFundCommandHandler : IAsyncRequestHandler<AddOrEditFundCommand, ICommandResult>
    {
        private readonly ApplicationDbContext context;
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public AddOrEditFundCommandHandler(ApplicationDbContext context, ISearch<Fund> funds)
        {
            this.context = context;
            this.funds = funds;
        }

        public async Task<ICommandResult> Handle(AddOrEditFundCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.FundId.HasValue)
                return await Edit(message);

            return new FailureResult("Add Fund is not implemented.");
        }

        private async Task<ICommandResult> Edit(AddOrEditFundCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.FundId);

            var fund = await funds.GetById(message.Editor.FundId.Value)
                .SingleOrDefaultAsync();

            message.Editor.UpdateFund(fund);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", fund.Id);

            return new SuccessResult(fund.Id);
        }
    }

    public class FundEditorMetaDataPostHandler : IAsyncPostRequestHandler<AddOrEditFundCommand, ICommandResult>
    {
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public FundEditorMetaDataPostHandler( ISearch<Fund> funds)
        {
            this.funds = funds;
        }

        public async Task Handle(AddOrEditFundCommand command, ICommandResult response)
        {
            Logger.Trace("Handle");

            if (response.IsSuccess)
                return;

            if (command.Editor.FundId.HasValue)
            {
                command.Editor.Fund = await funds.GetById(command.Editor.FundId.Value)
                    .SingleOrDefaultAsync();
            }
        }
    }
}