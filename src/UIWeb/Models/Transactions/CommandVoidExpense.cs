using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class VoidExpenseCommand:IAsyncRequest<ICommandResult>
    {
        public VoidExpenseEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public VoidExpenseCommand(VoidExpenseEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }
    public class VoidExpenseComanndHandler:IAsyncRequestHandler<VoidExpenseCommand, ICommandResult>
    {
        private readonly ISearch<Expense> expenses;

        public VoidExpenseComanndHandler(ISearch<Expense> expenses)
        {
            this.expenses = expenses;
        }

        public async Task<ICommandResult> Handle(VoidExpenseCommand message)
        {
            if (message.Editor.TransactionId.HasValue)
            {
                var expense = await expenses.GetById(message.Editor.TransactionId.Value)
                    .FirstOrDefaultAsync();
            }

            return await Task.FromResult(new SuccessResult(true));
        }
    }
}