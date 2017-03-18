using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategoryEditorFormQuery : IAsyncRequest<ExpenseCategoryEditorForm>
    {
        public int? ExpenseCategoryId { get; private set; }

        public ExpenseCategoryEditorFormQuery(int? expenseCategoryId = null)
        {
            this.ExpenseCategoryId = expenseCategoryId;
        }
    }

    public class ExpenseCategoryEditorFormQueryHandler : IAsyncRequestHandler<ExpenseCategoryEditorFormQuery, ExpenseCategoryEditorForm>
    {
        private readonly ISearch<ExpenseCategory> categories;
        public ILogger Logger { get; set; }

        public ExpenseCategoryEditorFormQueryHandler(ISearch<ExpenseCategory> categories)
        {
            this.categories = categories;
        }

        public async Task<ExpenseCategoryEditorForm> Handle(ExpenseCategoryEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.ExpenseCategoryId);

            if (message.ExpenseCategoryId.HasValue)
            {
                var category = await categories.GetById(message.ExpenseCategoryId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (category != null)
                    return ExpenseCategoryEditorForm.FromExpenseCategory(category);
            }

            return new ExpenseCategoryEditorForm();
        }
    }
}