using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class CheckDetails
    {
        public decimal Amount { get; set; }
        public int CheckNumber { get; set; }
        public string PaidTo { get; set; }
        public string Memo { get; set; }
        public string PrintedBy { get; set; }
        public string PrintedDate { get; set; }
        public Expense[] Expenses { get; set; }
    }
}
  





