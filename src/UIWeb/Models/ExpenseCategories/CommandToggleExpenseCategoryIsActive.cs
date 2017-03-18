using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ToggleExpenseCategoryIsActiveCommand : IAsyncRequest<ICommandResult>
    {
        public int ExpenseCategoryId { get; private set; }

        public ToggleExpenseCategoryIsActiveCommand(int expenseCategoryId)
        {
            this.ExpenseCategoryId = expenseCategoryId;
        }
    }

    public class ToggleExpenseCategoryIsActiveCommandHandler : IAsyncRequestHandler<ToggleExpenseCategoryIsActiveCommand, ICommandResult>
    {
        private readonly ISearch<ExpenseCategory> categories;
        private readonly IUnitOfWork unitOfWork;
        public ILogger Logger { get; set; }

        public ToggleExpenseCategoryIsActiveCommandHandler(ISearch<ExpenseCategory> categories, IUnitOfWork unitOfWork)
        {
            this.categories = categories;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult> Handle(ToggleExpenseCategoryIsActiveCommand command)
        {
            Logger.Trace("Handle::{0}", command.ExpenseCategoryId);

            var category = await categories.GetById(command.ExpenseCategoryId).FirstOrDefaultAsync();

            if( category == null)
                return new FailureResult("Expense Category {0} not found.".FormatWith(command.ExpenseCategoryId));

            category.IsActive = !category.IsActive;
            await unitOfWork.SaveChangesAsync();
            Logger.Info("Handle::Success Id:{0} IsActive:{1}", category.Id, category.IsActive);

            return new SuccessResult(category.Id);
        }
    }
}