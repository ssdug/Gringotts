using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class Check : IAmAuditable
    {
        public Check()
        {
            Created = Updated = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public int CheckNumber { get; set; }

        public string PaidTo { get; set; }

        public string Memo { get; set; }

        public string PrintedBy { get; set; }

        public int FundId { get; set; }

        [ForeignKey("FundId")]
        public virtual Fund Fund { get; set; }

        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }

        public int? TransactionId { get; set; }

        public virtual Expense Expense { get; set; }

        public int? TransactionBatchId { get; set; }

        public virtual ExpenseBatch ExpenseBatch {get; set;}

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedBy { get; set; }
    }
}