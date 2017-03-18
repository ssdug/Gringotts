using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategoryDetailsQuery : IAsyncRequest<ExpenseCategoryDetails>
    {
        public int ExpenseCategoryId { get; private set; }

        public ExpenseCategoryDetailsQuery(int expenseCategoryId)
        {
            this.ExpenseCategoryId = expenseCategoryId;
        }
    }

    public class ExpenseCategoryDetailsQueryHandler : IAsyncRequestHandler<ExpenseCategoryDetailsQuery, ExpenseCategoryDetails>
    {
        private readonly ISearch<ExpenseCategory> categories;
        public ILogger Logger { get; set; }

        public ExpenseCategoryDetailsQueryHandler(ISearch<ExpenseCategory> categories)
        {
            this.categories = categories;
        }

        public async Task<ExpenseCategoryDetails> Handle(ExpenseCategoryDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.ExpenseCategoryId);
            
            var category = await categories.GetById(query.ExpenseCategoryId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (category != null)
            {
                return new ExpenseCategoryDetails
                {
                    ExpenseCategory = category
                };
            }

            return null;
        }
    }
}