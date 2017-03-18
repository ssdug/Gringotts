using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountType : IAmLookupItem
    {
        public static readonly string Checking = "Checking";
        public static string Savings = "Savings";
        public static string Restitution = "Restitution";

        public int Id { get; set; }

        [Required, Index(IsUnique = true),
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public ClientAccount BuildClientAccount(Client client, Organization organization)
        {
            return new ClientAccount
            {
                Residency = client.Residencies.First(r => r.OrganizationId == organization.Id),
                Name = this.Name,
                AccountType = this
            };
        }
    }

    public abstract class Account : IAmAuditable, IBelongToOrganization, ICanBeActive
    {
        protected Account()
        {
            Transactions = new HashSet<Transaction>();
            Created = Updated = DateTime.UtcNow;
            IsActive = true;
        }

        public int Id { get; set; }

        [MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [MaxLength(255), MinLength(1)]
        public string BankNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public int FundId { get; set; }

        [ForeignKey("FundId")]
        public virtual Fund Fund { get; set; }

        public int AccountTypeId { get; set; }

        [ForeignKey("AccountTypeId")]
        public virtual AccountType AccountType { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Encumbered { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Available { get; set; }

        public bool IsActive { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }

        public void AddReceipt(Receipt receipt)
        {
            this.Transactions.Add(receipt);
            this.Available += receipt.Amount;
            this.Total += receipt.Amount;
            this.Fund.Available += receipt.Amount;
            this.Fund.Total += receipt.Amount;
        }

        public void RemoveReceipt(Receipt receipt)
        {
            Transactions.Remove(receipt);
            this.Available -= receipt.Amount;
            this.Total -= receipt.Amount;
            this.Fund.Available -= receipt.Amount;
            this.Fund.Total -= receipt.Amount;
        }

        public void AddExpense(Expense expense)
        {
            this.Transactions.Add(expense);
            this.Available -= expense.Amount;
            this.Total -= expense.Amount;
            this.Fund.Available -= expense.Amount;
            this.Fund.Total -= expense.Amount;
        }

        public void RemoveExpense(Expense expense)
        {
            this.Transactions.Remove(expense);
            this.Available += expense.Amount;
            this.Total += expense.Amount;
            this.Fund.Available += expense.Amount;
            this.Fund.Total += expense.Amount;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(BankNumber))
                return Name;

            return "{0} - {1}".FormatWith(Name, BankNumber);
        }
    }

    public class SubsidiaryAccount : Account
    { }

    public class ClientAccount : Account
    {
        public int ResidencyId { get; set; }

        [ForeignKey("ResidencyId")]
        public virtual Residency Residency { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(BankNumber))
                return "{0}, {1} {2} {3}".FormatWith(Residency.Client.LastName, Residency.Client.FirstName, Residency.Client.MiddleName, Name);

            return "{0}, {1} {2} {3} - {4}".FormatWith(Residency.Client.LastName, Residency.Client.FirstName, Residency.Client.MiddleName, Name, BankNumber);
        }
    }
}