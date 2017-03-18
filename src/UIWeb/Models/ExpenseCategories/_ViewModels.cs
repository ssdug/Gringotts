using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategoryDetails
    {
        public ExpenseCategory ExpenseCategory { get; set; }
    }

    public class ExpenseCategorySearchResult : IPagedSearchResult<ExpenseCategory>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<ExpenseCategory> Items { get; set; }
    }
}