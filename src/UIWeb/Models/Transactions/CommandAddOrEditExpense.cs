using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class AddOrEditExpenseCommand : IAsyncRequest<ICommandResult>
    {
        public ExpenseEditorForm Editor { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        public AddOrEditExpenseCommand(ExpenseEditorForm editor, ModelStateDictionary modelState)
        {
            Editor = editor;
            ModelState = modelState;
        }
    }

    public class AddOrEditExpenseCommandHandler : IAsyncRequestHandler<AddOrEditExpenseCommand, ICommandResult>
    {
        private readonly ISearch<Account> accounts;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditExpenseCommandHandler(ISearch<Account> accounts, ApplicationDbContext context)
        {
            this.accounts = accounts;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditExpenseCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

                return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditExpenseCommand message)
        {
            Logger.Trace("Add");

            var account = await accounts.GetById(message.Editor.AccountId.Value)
                .FirstOrDefaultAsync();

            var expense = message.Editor.BuildExpense(account, context);

            account.AddExpense(expense);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", expense.Id);

            return new SuccessResult(expense.Id);
        }
    }

    public class AddOrEditExpenseRestitutionOrderPostProcessor : IAsyncPostRequestHandler<AddOrEditExpenseCommand, ICommandResult>
    {
        private readonly ISearch<Order> orders;
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditExpenseRestitutionOrderPostProcessor(ISearch<Order> orders, ApplicationDbContext context)
        {
            this.orders = orders;
            this.context = context;
        }

        public async Task Handle(AddOrEditExpenseCommand command, ICommandResult response)
        {
            Logger.Trace("Handle");

            if (response.IsFailure)
                return;

            if (!command.Editor.OrderId.HasValue)
                return;

            var order = await orders.GetById(command.Editor.OrderId.Value)
                .FirstAsync();

            order.Balance -= command.Editor.Amount.ToNullableDecimal().Value;

            await context.SaveChangesAsync();
        }
    }
}