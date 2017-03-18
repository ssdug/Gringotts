using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models.Funds;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountEditorForm
    {
        public Fund ParentFund { get; set; }
        public int? ParentFundId { get; set; }

        public Account Account { get; set; }
        public int? AccountId { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [MaxLength(255), MinLength(1)]
        public string BankNumber { get; set; }

        public string Comments { get; set; }
        public bool CanEditName { get { return ParentFund is SubsidiaryFund; } }

        public static AccountEditorForm FromFund(Fund fund, Account account = null)
        {
            var form = new AccountEditorForm();

            if (fund != null)
            {
                form.ParentFund = fund;
                form.ParentFundId = fund.Id;
            }

            if (account != null)
            {
                form.Account = account;
                form.AccountId = account.Id;
                form.Name = account.Name;
                form.BankNumber = account.BankNumber;
                form.Comments = account.Comments;
            }

            return form;
        }

        public SubsidiaryAccount BuildAccount(SubsidiaryFund fund, ILookup<AccountType> lookup)
        {
            return new SubsidiaryAccount
            {
                Name = this.Name,
                BankNumber = this.BankNumber,
                Comments = this.Comments,
                AccountType = lookup.All.FirstOrDefault(t => t.Name.Equals(AccountType.Checking)),
                Fund = fund
            };
        }

        public void UpdateAccount(Account account)
        {
            if (account is SubsidiaryAccount)
                account.Name = this.Name;

            account.BankNumber = this.BankNumber;
            account.Comments = this.Comments;
        }
    }
}