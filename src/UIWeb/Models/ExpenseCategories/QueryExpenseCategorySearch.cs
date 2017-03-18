using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategorySearchQuery : IAsyncRequest<ExpenseCategorySearchResult>
    {
        public SearchPager Pager { get; private set; }

        public ExpenseCategorySearchQuery(SearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class ExpenseCategorySearchQueryHandler : IAsyncRequestHandler<ExpenseCategorySearchQuery, ExpenseCategorySearchResult>
    {
        private readonly ISearch<ExpenseCategory> categories;
        public ILogger Logger { get; set; }

        public ExpenseCategorySearchQueryHandler(ISearch<ExpenseCategory> categories)
        {
            this.categories = categories;
        }

        public async Task<ExpenseCategorySearchResult> Handle(ExpenseCategorySearchQuery message)
        {
            Logger.Trace("Handle");

            var items = categories.GetBySearch(message.Pager)
                .OrderBy(c => c.Name)
                .AsNoTracking();

            return new ExpenseCategorySearchResult
            {
                Pager = message.Pager,
                Items = await items.ToPagedListAsync(message.Pager.Page, message.Pager.PageSize)
            };
        }
    }
}