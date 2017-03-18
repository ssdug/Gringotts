using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Autofac.Integration.Mvc;
using Wiz.Gringotts.UIWeb.Binders;
using MvcValidationExtensions.Attribute;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public partial class ReceiptEditorForm
    {
        public ReceiptEditorForm()
        {
            this.EffectiveDate = DateTime.Today;
        }
        public bool IsClientAccount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public string Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        public int ReceiptTypeId { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string ReceivedFrom { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceivedFor { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceiptNumber { get; set; }

        public int? ParentFundId { get; set; }

        public int? AccountId { get; set; }

        //typeahead inputs
        [RequiredIf("ReceiptSourceName", AllowEmptyStrings = true, ErrorMessage = "Invalid Receipt Source")]
        public int? ReceiptSourceId { get; set; }

        [Required]
        public string ReceiptSourceName { get; set; }
        //end typeahead inputs

        public ReceiptType[] AvailableTypes { get; set; }
    }

    public partial class ExpenseEditorForm
    {
        public ExpenseEditorForm()
        {
            this.EffectiveDate = DateTime.Today;
        }

        public bool IsClientAccount { get; set; }
        public int? OrderId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public string Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? Available { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        public int ExpenseTypeId { get; set; }

        //typeahead inputs
        [RequiredIf("PayeeName", AllowEmptyStrings = true, ErrorMessage = "Invalid Payee")]
        public int? PayeeId { get; set; }

        [Required]
        public string PayeeName { get; set; }

        [RequiredIf("ExpenseCategoryName", AllowEmptyStrings = true, ErrorMessage = "Invalid Expense Category")]
        public int? ExpenseCategoryId { get; set; }

        [Required]
        public string ExpenseCategoryName { get; set; }
        //end typeahead inputs

        [Required, MaxLength(255), MinLength(1)]
        public string Memo { get; set; }

        public int? AccountId { get; set; }
        public int? ParentFundId { get; set; }
        public ExpenseType[] AvailableTypes { get; set; }
    }

    public abstract partial class BatchExpenseEditorForm
    {
        [ModelBinderType(typeof(BatchExpenseEditorForm))]
        public class BatchExpenseEditorFormModelBinder : PolymorphicModelBinder
        {
            protected override Type GetBaseType()
            {
                return typeof(BatchExpenseEditorForm);
            }
        }

        public BatchExpenseEditorForm()
        {
            this.EffectiveDate = DateTime.Today.Date;
            this.AvailableTypes = new List<ExpenseType>();
        }

        public int? BatchId { get; set; }

        public int FundId { get; set; }

        public string ReferenceNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? ExpectedAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EffectiveDate { get; set; }

        public int ExpenseTypeId { get; set; }

        //typeahead inputs
        [RequiredIf("PayeeName", AllowEmptyStrings = true, ErrorMessage = "Invalid Payee")]
        public int PayeeId { get; set; }

        [Required]
        public string PayeeName { get; set; }
        //end typeahead inputs


        [Required, MaxLength(255), MinLength(1)]
        public string Memo { get; set; }

        public IList<ExpenseType> AvailableTypes { get; set; }

        public string ConcreteModelType { get { return this.GetType().ToString(); } }
 
    }

    public partial class SubsidiaryBatchExpenseEditorForm : BatchExpenseEditorForm
    {
        public SubsidiaryBatchExpenseEditorForm()
        {
            Transactions = new List<SubsidiaryExpense>();
        }

        public IList<SubsidiaryExpense> Transactions { get; set; } 
    }

    public partial class SubsidiaryExpense
    {
        public int? Id { get; set; }
        public string ReferenceNumber { get; set; }

        //typeahead inputs
        [RequiredIf("AccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid Account")]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [RequiredIf("ExpenseCategoryName", AllowEmptyStrings = true, ErrorMessage = "Invalid Expense Category")]
        public int ExpenseCategoryId { get; set; }

        [Required]
        public string ExpenseCategoryName { get; set; }
        //end typeahead inputs

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(InputRegEx.Currency,
            ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? Amount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }
 
    }

    public partial class ClientBatchExpenseEditorForm : BatchExpenseEditorForm
    {
        public ClientBatchExpenseEditorForm()
        {
            Transactions = new List<ClientExpense>();
        }

        public IList<ClientExpense> Transactions { get; set; } 
    }

    public partial class ClientExpense
    {
        public int? Id { get; set; }
        public string ReferenceNumber { get; set; }

        //typeahead inputs
        [RequiredIf("AccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid Account")]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [RequiredIf("ClientName", AllowEmptyStrings = true, ErrorMessage = "Invalid Client")]
        public int ClientId { get; set; }

        [Required]
        public string ClientName { get; set; }

        [RequiredIf("ExpenseCategoryName", AllowEmptyStrings = true, ErrorMessage = "Invalid Expense Category")]
        public int ExpenseCategoryId { get; set; }

        [Required]
        public string ExpenseCategoryName { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]

        [RegularExpression(InputRegEx.Currency,
       ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? Amount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        
    }

    public abstract partial class BatchReceiptEditorForm
    {
        [ModelBinderType(typeof(BatchReceiptEditorForm))]
        public class BatchExpenseEditorFormModelBinder : PolymorphicModelBinder
        {
            protected override Type GetBaseType()
            {
                return typeof(BatchReceiptEditorForm);
            }
        }

        public BatchReceiptEditorForm()
        {
            this.EffectiveDate = DateTime.Today.Date;
            this.AvailableTypes = new List<ReceiptType>();
        }

        public int? BatchId { get; set; }

        public int FundId { get; set; }

        public string ReferenceNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? ExpectedAmount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EffectiveDate { get; set; }

        public int ReceiptTypeId { get; set; }
        
        public IList<ReceiptType> AvailableTypes { get; set; }

        public string ConcreteModelType { get { return this.GetType().ToString(); } }
    }

    public partial class SubsidiaryBatchReceiptEditorForm : BatchReceiptEditorForm
    {
        public SubsidiaryBatchReceiptEditorForm()
        {
            Transactions = new List<SubsidiaryReceipt>();
        }
        public IList<SubsidiaryReceipt> Transactions { get; set; }
    }

    public partial class SubsidiaryReceipt
    {
        public int? Id { get; set; }
        public string ReferenceNumber { get; set; }

        //typeahead inputs
        [RequiredIf("AccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid Account")]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [RequiredIf("ReceiptSourceName", AllowEmptyStrings = true, ErrorMessage = "Invalid Receipt Source")]
        public int ReceiptSourceId { get; set; }

        [Required]
        public string ReceiptSourceName { get; set; }
        //end typeahead inputs

        [Required, MaxLength(255), MinLength(1)]
        public string ReceivedFrom { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceivedFor { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceiptNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(InputRegEx.Currency,
            ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? Amount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }

    public partial class ClientBatchReceiptEditorForm : BatchReceiptEditorForm
    {
        public ClientBatchReceiptEditorForm()
        {
            Transactions = new List<ClientReceipt>();
        }
        public IList<ClientReceipt> Transactions { get; set; }
    }

    public partial class ClientReceipt
    {
        public int? Id { get; set; }
        public string ReferenceNumber { get; set; }

        //typeahead inputs
        [RequiredIf("AccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid Account")]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [RequiredIf("ClientName", AllowEmptyStrings = true, ErrorMessage = "Invalid Client")]
        public int ClientId { get; set; }

        [Required]
        public string ClientName { get; set; }

        [RequiredIf("ReceiptSourceName", AllowEmptyStrings = true, ErrorMessage = "Invalid Receipt Source")]
        public int ReceiptSourceId { get; set; }

        [Required]
        public string ReceiptSourceName { get; set; }
        //end typeahead inputs

        [Required, MaxLength(255), MinLength(1)]
        public string ReceivedFrom { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceivedFor { get; set; }

        [MaxLength(255), MinLength(1)]
        public string ReceiptNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [RegularExpression(InputRegEx.Currency,
            ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? Amount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }
    }

    public class TrasferAmountEditorForm
    {
        public int? ParentBatchId { get; set; }
        public int? TriggerTransactionId { get; set; }
        public int? TriggerTransactionAccountId { get; set; }

        public int? FundId { get; set; }
        public int? ClientId { get; set; }

        [RequiredIf("FromAccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid From Account")]
        public int FromAccountId { get; set; }

        [Required]
        public string FromAccountName { get; set; }

        [RequiredIf("ToAccountName", AllowEmptyStrings = true, ErrorMessage = "Invalid To Account")]
        public int ToAccountId { get; set; }

        [Required]
        public string ToAccountName { get; set; }

        public int PayeeId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal Amount { get; set; }
    }

    public class VoidExpenseEditorForm
    {
        public int? AccountId { get; set; }
        public int? TransactionId { get; set; }

        public int? VoidTypeId { get; set; }

        [Display(Name = "Comments")]
        [RequiredIfValue("VoidTypeId","20")]
        public string Comments { get; set; }

        public VoidType[] AvailableTypes { get; set; }

    }
}