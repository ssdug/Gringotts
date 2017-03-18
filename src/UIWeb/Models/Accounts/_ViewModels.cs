using System.Collections.Generic;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Wiz.Gringotts.UIWeb.Models.Users;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountDetails : IPagedSearchResult<Transaction>
    {
        public Fund Fund { get; set; }
        public Account Account { get; set; }
        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
        public SearchPager Pager { get; set; }
        public IPagedList<Transaction> Items { get; set; }


        public VoidExpenseEditorForm VoidExpenseEditorForm { get; set; }
    }
}