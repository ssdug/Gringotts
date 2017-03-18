using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class AddOrEditReceiptCommand : IAsyncRequest<ICommandResult>
    {
        public ReceiptEditorForm Editor { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        public AddOrEditReceiptCommand(ReceiptEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class AddOrEditReceiptCommandHandler : IAsyncRequestHandler<AddOrEditReceiptCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Account> accounts;
        private readonly ApplicationDbContext context;
        private readonly IMediator mediator;

        public AddOrEditReceiptCommandHandler(ISearch<Account> accounts, ApplicationDbContext context, IMediator mediator)
        {
            this.accounts = accounts;
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<ICommandResult> Handle(AddOrEditReceiptCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditReceiptCommand command)
        {
            Logger.Trace("Add");

            var account = await accounts.GetById(command.Editor.AccountId.Value)
                .FirstOrDefaultAsync();

            var receipt = command.Editor.BuildReceipt(account, context);

            account.AddReceipt(receipt);

           
            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", receipt.Id);

            await mediator.PublishAsync(new ReceiptAddedNotification(receipt, addOrEditReceiptCommand: command));

            return new SuccessResult(receipt.Id);
        }
    }
}