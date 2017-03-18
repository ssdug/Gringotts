using System.ComponentModel.DataAnnotations;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundEditorForm
    {
        public Fund Fund { get; set; }
        public int? FundId { get; set; }

        [MaxLength(255), MinLength(1)]
        public string BankNumber { get; set; }

        public string Comments { get; set; }

        public static FundEditorForm FromFund(Fund fund)
        {
            return new FundEditorForm
            {
                Fund = fund,
                FundId = fund.Id,
                BankNumber = fund.BankNumber,
                Comments = fund.Comments
            };
        }

        public void UpdateFund(Fund fund)
        {
            fund.BankNumber = this.BankNumber;
            fund.Comments = this.Comments;
        }
    }
}