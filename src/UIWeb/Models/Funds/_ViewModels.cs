using System.Collections.Generic;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundDetails 
    {
        public Fund Fund { get; set; }
        public SearchPager Pager { get; set; }
        public IPagedList<FundSubsidiary> Items { get; set; }
        public IPagedList<TransactionBatch> Batches { get; set; }
    }

    public class FundSubsidiary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public decimal Encumbered { get; set; }
        public decimal Available { get; set; }
        public IEnumerable<string> BankNumbers { get; set; }
    }

    public class FundClientSubsidiary : FundSubsidiary
    {
        public IEnumerable<Account> Accounts { get; set; }

        public Client Client
        {
            get
            {
                return this.Accounts.Cast<ClientAccount>()
                    .Select(a => a.Residency.Client)
                    .FirstOrDefault();
            }
        }
    }

    public class FundsSearchResult : IPagedSearchResult<Fund>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<Fund> Items { get; set; }
    }
}