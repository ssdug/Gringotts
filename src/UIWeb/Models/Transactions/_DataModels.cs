using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.ExpenseCategories;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Wiz.Gringotts.UIWeb.Models.ReceiptSources;
using Wiz.Gringotts.UIWeb.Models.Checks;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public abstract class TransactionBatch : IAmAuditable, IBelongToOrganization
    {
        public TransactionBatch()
        {
            Created = Updated = Effective = DateTime.UtcNow;
            Children = new List<TransactionBatch>();
        }

        public int Id { get; set; }

        public string BatchReferenceNumber { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal ExpectedAmount { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public bool Committed { get; set; }

        public DateTime Effective { get; set; }
        
        public int? TransactionSubTypeId { get; set; }

        [ForeignKey("TransactionSubTypeId")]
        public virtual TransactionSubType TransactionSubType { get; set; }


        public int? TriggerId { get; set; }
        public int? TriggerAccountId { get; set; }

        public virtual Transaction Trigger { get; set; }


        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]

        public virtual TransactionBatch Parent { get; set; }

        public virtual ICollection<TransactionBatch> Children { get; set; } 

        public int FundId { get; set; }

        [ForeignKey("FundId")]
        public virtual Fund Fund { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class ExpenseBatch : TransactionBatch
    {
        [NotMapped]
        public virtual ExpenseType ExpenseType
        {
            get { return this.TransactionSubType as ExpenseType; }
            set { this.TransactionSubType = value; }
        }

        [Required, MaxLength(255), MinLength(1)]
        public string Memo { get; set; }

        public int PayeeId { get; set; }

        [ForeignKey("PayeeId")]
        public virtual Payee Payee { get; set; }

        public virtual ICollection<Check> Checks { get; set; }

    }

    public class ReceiptBatch : TransactionBatch
    {
        [NotMapped]
        public virtual ReceiptType ReceiptType
        {
            get { return this.TransactionSubType as ReceiptType; }
            set { this.TransactionSubType = value; }
        }
    }

    public class TransferBatch : TransactionBatch
    {
        public int FromAccountId { get; set; }
        [ForeignKey("FromAccountId")]
        public virtual Account FromAccount { get; set; }

        public int ToAccountId { get; set; }
        [ForeignKey("ToAccountId")]
        public virtual Account ToAccount { get; set; }
    }

    public abstract class Transaction : IAmAuditable, IBelongToOrganization
    {
        protected Transaction()
        {
            Created = Updated = Effective = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public string BatchReferenceNumber { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public DateTime Effective { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public int? TransactionBatchId { get; set; }

        [ForeignKey("TransactionBatchId")]
        public virtual TransactionBatch Batch { get; set; }

        public int TransactionSubTypeId { get; set; }

        [ForeignKey("TransactionSubTypeId")]
        public virtual TransactionSubType TransactionSubType { get; set; }

        public int FundId { get; set; }

        [ForeignKey("FundId")]
        public virtual Fund Fund { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

    public abstract class TransactionSubType : IAmLookupItem
    {
        public int Id { get; set; }

        [Required, Index,
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public bool UserSelectable { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ReceiptType : TransactionSubType
    {
        public const string Seed = "Seed";
        public const string Cash = "Cash";
        public const string Coin = "Coin";
        public const string Check = "Check";
        public const string Payroll = "Payroll";
        public const string EFT = "EFT";
        public const string MoneyOrder = "Money Order";
        public const string Transfer = "Transfer";
    }

    public class Receipt : Transaction
    {

        [NotMapped]
        public virtual ReceiptType ReceiptType
        {
            get { return this.TransactionSubType as ReceiptType; }
            set { this.TransactionSubType = value; }
        }

        [Required, MaxLength(255), MinLength(1)]
        public string ReceivedFrom { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceivedFor { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceiptNumber { get; set; }

        public int? ReceiptSourceId { get; set; }

        [ForeignKey("ReceiptSourceId")]
        public virtual ReceiptSource ReceiptSource { get; set; }
    }

    public class ExpenseType : TransactionSubType
    {
        public const string Cash = "Cash";
        public const string Check = "Check";
        public const string Card = "Visa Expense Card";
        public const string PurchaseOrder = "Purchase Order";
        public const string Transfer = "Transfer";
    }

    public class Expense : Transaction
    {
        public Expense()
        {
            this.Checks = new List<Check>();
        }

        [NotMapped]
        public virtual ExpenseType ExpenseType
        {
            get { return this.TransactionSubType as ExpenseType; }
            set { this.TransactionSubType = value; }
        }

        [Required, MaxLength(255), MinLength(1)]
        public string Memo { get; set; }

        public int PayeeId { get; set; }

        [ForeignKey("PayeeId")]
        public virtual Payee Payee { get; set; }


        public int? ExpenseCategoryId { get; set; }

        [ForeignKey("ExpenseCategoryId")]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

        public virtual ICollection<Check> Checks { get; set; }
    }

    public class VoidType : TransactionSubType
    {
        public const string Duplicate = "Duplicate";
        public const string WrongAccount = "Wrong Account/Client";
        public const string WrongVendor = "Wrong Vendor";
        public const string WrongAmount = "Wrong Amount";
        public const string SpoiledDamaged = "Spoiled/Damaged";
        public const string LostStolen = "Lost/Stolen";
        public const string Other = "Other";
    }
}