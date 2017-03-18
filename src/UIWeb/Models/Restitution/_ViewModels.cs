using Wiz.Gringotts.UIWeb.Models.Transactions;
using Wiz.Gringotts.UIWeb.Models.Users;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrderDetails : IPagedSearchResult<Transaction>
    {
        public Order Order { get; set; }

        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
        public SearchPager Pager { get; set; }
        public IPagedList<Transaction> Items { get; set; }
    }
}