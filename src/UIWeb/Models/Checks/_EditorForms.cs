using System;
using System.ComponentModel.DataAnnotations;
using Wiz.Gringotts.UIWeb.Models.Transactions;


namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class CheckEditorForm
    {
        public int? CheckId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public string Amount { get; set; }

        [MaxLength(255), MinLength(1)]
        public string PaidTo { get; set; }

        [Required]
        [RegularExpression(InputRegEx.Interger,
           ErrorMessage = "Enter a valid integer value")]
        public string CheckNumber { get; set; }
        public string Memo { get; set; }
        public string PrintedBy { get; set; }
        public string PreparedBy { get; set; }
        public int? TransactionId { get; set; }
        public int? AccountId { get; set; }
        public int? TransactionBatchId { get; set; }
        public int FundId { get; set; }
        public bool WasPrinted { get; set; }
        public Check BuildCheck()
        {
            return new Check
            {
                Amount = Convert.ToDecimal(this.Amount),
                CheckNumber = Convert.ToInt32(this.CheckNumber),
                TransactionId = this.TransactionId,
                AccountId = this.AccountId,
                TransactionBatchId = this.TransactionBatchId,
                FundId = this.FundId,
                PaidTo = this.PaidTo,
                Memo = this.Memo,
                PrintedBy = this.PreparedBy,
                Created = DateTime.Today
            };
        }
    }
}