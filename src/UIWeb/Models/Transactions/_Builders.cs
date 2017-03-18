using System;
using System.Linq;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Accounts;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public partial class ReceiptEditorForm
    {
        public Receipt BuildReceipt(Account account, ApplicationDbContext context)
        {
            var effectiveDate = EffectiveDate.HasValue ?
                EffectiveDate.Value.ToUniversalTime() :
                (DateTime?)null;

            return new Receipt
            {
                Amount = Convert.ToDecimal(this.Amount),
                Effective = effectiveDate ?? DateTime.UtcNow,
                Comments = this.Comments,
                ReceivedFrom = this.ReceivedFrom,
                ReceivedFor = this.ReceivedFor,
                ReceiptNumber = this.ReceiptNumber,
                ReceiptSourceId = ReceiptSourceId,
                FundId = account.FundId,
                AccountId = account.Id,
                TransactionSubType = context.TransactionSubTypes.First(t => t.Id == ReceiptTypeId)   
            };
        }
    }

    public partial class BatchReceiptEditorForm
    {
        public ReceiptBatch BuildBatch(ApplicationDbContext context)
        {
            return new ReceiptBatch
            {
                FundId = FundId,
                TransactionSubTypeId = ReceiptTypeId,
                ExpectedAmount = ExpectedAmount.GetValueOrDefault(),
                Effective = EffectiveDate.Date.ToUniversalTime()
            };
        }
    }

    public partial class SubsidiaryBatchReceiptEditorForm
    {
        public static SubsidiaryBatchReceiptEditorForm FromBatch(ReceiptBatch batch, Receipt[] transactions, ReceiptType[] receiptTypes)
        {
            return new SubsidiaryBatchReceiptEditorForm
            {
                BatchId = batch.Id,
                ReferenceNumber = batch.BatchReferenceNumber,
                FundId = batch.FundId,
                AvailableTypes = receiptTypes,
                ReceiptTypeId = batch.ReceiptType.Id,
                ExpectedAmount = batch.ExpectedAmount,
                TotalAmount = transactions.Sum(t => t.Amount),
                EffectiveDate = batch.Effective.ToUniversalTime(),
                Transactions = transactions.Select(SubsidiaryReceipt.FromTransaction)
                .ToArray()
            };
        }
    }

    public partial class SubsidiaryReceipt
    {
        public Receipt BuildReceipt(ReceiptBatch batch, ApplicationDbContext context)
        {
            return new Receipt
            {
                FundId = batch.FundId,
                AccountId = AccountId,
                TransactionBatchId = batch.Id,
                ReceiptSourceId = ReceiptSourceId,
                TransactionSubTypeId = batch.TransactionSubTypeId.Value,
                ReceiptNumber = this.ReceiptNumber,
                ReceivedFor = this.ReceivedFor,
                ReceivedFrom = this.ReceivedFrom,
                Effective = batch.Effective.ToUniversalTime(),
                Comments = Comments,
                Amount = Amount.GetValueOrDefault()
            };
        }

        public static SubsidiaryReceipt FromTransaction(Receipt receipt)
        {
            return new SubsidiaryReceipt
            {
                Id = receipt.Id,
                ReferenceNumber = receipt.BatchReferenceNumber,
                AccountId = receipt.AccountId,
                AccountName = receipt.Account.Name,
                ReceiptSourceId = receipt.ReceiptSourceId.Value,
                ReceiptSourceName = receipt.ReceiptSource.Name,
                ReceiptNumber = receipt.ReceiptNumber,
                ReceivedFor = receipt.ReceivedFor,
                ReceivedFrom = receipt.ReceivedFrom,
                Amount = receipt.Amount,
                Comments = receipt.Comments
            };
        }
    }


    public partial class ClientBatchReceiptEditorForm
    {
        public static ClientBatchReceiptEditorForm FromBatch(ReceiptBatch batch, Receipt[] transactions, ReceiptType[] receiptTypes)
        {
            return new ClientBatchReceiptEditorForm
            {
                BatchId = batch.Id,
                ReferenceNumber = batch.BatchReferenceNumber,
                FundId = batch.FundId,
                AvailableTypes = receiptTypes,
                ReceiptTypeId = batch.ReceiptType.Id,
                ExpectedAmount = batch.ExpectedAmount,
                TotalAmount = transactions.Sum(t => t.Amount),
                EffectiveDate = batch.Effective,
                Transactions = transactions.Select(ClientReceipt.FromTransaction)
                .ToArray()
            };
        }
    }

    public partial class ClientReceipt
    {

        public Receipt BuildReceipt(ReceiptBatch batch, ApplicationDbContext context)
        {
            return new Receipt
            {
                FundId = batch.FundId,
                AccountId = AccountId,
                TransactionBatchId = batch.Id,
                TransactionSubTypeId = batch.TransactionSubTypeId.Value,
                ReceiptSourceId = ReceiptSourceId,
                ReceivedFor = ReceivedFor,
                ReceivedFrom = ReceivedFrom,
                ReceiptNumber = ReceiptNumber,
                Amount = Amount.GetValueOrDefault(),
                Comments = Comments
            };
        }
        public static ClientReceipt FromTransaction(Receipt receipt)
        {
            var account = receipt.Account as ClientAccount;
            var residency = account.Residency;
            var client = residency.Client;

            return new ClientReceipt
            {
                Id = receipt.Id,
                ReferenceNumber = receipt.BatchReferenceNumber,
                ClientId = client.Id,
                ClientName = client.DisplayName,
                AccountId = receipt.AccountId,
                AccountName = receipt.Account.Name,
                ReceiptSourceId = receipt.ReceiptSourceId.GetValueOrDefault(),
                ReceiptSourceName = receipt.ReceiptSource.Name,
                ReceiptNumber = receipt.ReceiptNumber,
                ReceivedFor = receipt.ReceivedFor,
                ReceivedFrom = receipt.ReceivedFrom,
                Amount = receipt.Amount,
                Comments = receipt.Comments
            };
        }
    }


    public partial class ExpenseEditorForm
    {
        public Expense BuildExpense(Account account, ApplicationDbContext context)
        {
            var effectiveDate = EffectiveDate.HasValue ? 
                EffectiveDate.Value.ToUniversalTime() : 
                (DateTime?) null;

            return new Expense
            {
                Amount = Convert.ToDecimal(this.Amount),
                Effective = effectiveDate ?? DateTime.UtcNow,
                Fund = account.Fund,
                Account = account,
                Memo = this.Memo,
                Comments = this.Comments,
                PayeeId = PayeeId.Value,
                TransactionSubTypeId = this.ExpenseTypeId,
                ExpenseCategoryId = this.ExpenseCategoryId
            };
        }
    }

    public abstract partial class BatchExpenseEditorForm
    {
        public ExpenseBatch BuildBatch(ApplicationDbContext context)
        {
            return new ExpenseBatch
            {
                FundId = this.FundId,
                TransactionSubTypeId = this.ExpenseTypeId,
                PayeeId = this.PayeeId,
                ExpectedAmount = this.ExpectedAmount.GetValueOrDefault(),
                Effective = EffectiveDate.Date.ToUniversalTime(),
                Memo = this.Memo
            };
        }
    }

    public partial class SubsidiaryBatchExpenseEditorForm
    {
        public static SubsidiaryBatchExpenseEditorForm FromBatch(ExpenseBatch batch, Expense[] transactions, ExpenseType[] expenseTypes)
        {
            return new SubsidiaryBatchExpenseEditorForm
            {
                BatchId = batch.Id,
                ReferenceNumber = batch.BatchReferenceNumber,
                FundId = batch.FundId,
                AvailableTypes = expenseTypes,
                ExpenseTypeId = batch.ExpenseType.Id,
                PayeeId = batch.PayeeId,
                PayeeName = batch.Payee.Name,
                ExpectedAmount = batch.ExpectedAmount,
                TotalAmount = transactions.Sum(t => t.Amount),
                EffectiveDate = batch.Effective,
                Memo = batch.Memo,
                Transactions = transactions.Select(SubsidiaryExpense.FromTransaction)
                .ToArray()
            };
        }
    }

    public partial class SubsidiaryExpense
    {
        public Expense BuildExpense(ExpenseBatch batch, ApplicationDbContext context)
        {
            return new Expense
            {
                FundId = batch.FundId,
                AccountId = this.AccountId,
                TransactionBatchId = batch.Id,
                ExpenseCategoryId = this.ExpenseCategoryId,
                TransactionSubTypeId = batch.TransactionSubTypeId.Value,
                PayeeId = batch.PayeeId,
                Effective = batch.Effective,
                Memo = "Batch Expense",
                Comments = this.Comments,
                Amount = this.Amount.GetValueOrDefault()
            };
        }

        public static SubsidiaryExpense FromTransaction(Expense expense)
        {
            return new SubsidiaryExpense
            {
                Id = expense.Id,
                ReferenceNumber = expense.BatchReferenceNumber,
                AccountId = expense.AccountId,
                AccountName = expense.Account.Name,
                ExpenseCategoryId = expense.ExpenseCategoryId.GetValueOrDefault(),
                ExpenseCategoryName = expense.ExpenseCategory.Name,
                Amount = expense.Amount,
                Comments = expense.Comments
            };
        }
    }

    public partial class ClientBatchExpenseEditorForm
    {
        public static ClientBatchExpenseEditorForm FromBatch(ExpenseBatch batch, Expense[] transactions, ExpenseType[] expenseTypes)
        {
            return new ClientBatchExpenseEditorForm
            {
                BatchId = batch.Id,
                ReferenceNumber = batch.BatchReferenceNumber,
                FundId = batch.FundId,
                AvailableTypes = expenseTypes,
                ExpenseTypeId = batch.ExpenseType.Id,
                PayeeId = batch.PayeeId,
                PayeeName = batch.Payee.Name,
                ExpectedAmount = batch.ExpectedAmount,
                TotalAmount = transactions.Sum(t => t.Amount),
                EffectiveDate = batch.Effective,
                Memo = batch.Memo,
                Transactions = transactions.Select(ClientExpense.FromTransaction)
                .ToArray()
            };
        }
    }

    public partial class ClientExpense
    {
        public Expense BuildExpense(ExpenseBatch batch, ApplicationDbContext context)
        {
            return new Expense
            {
                FundId = batch.FundId,
                AccountId = this.AccountId,
                TransactionBatchId = batch.Id,
                TransactionSubTypeId = batch.TransactionSubTypeId.Value,
                ExpenseCategoryId = this.ExpenseCategoryId,
                PayeeId = batch.PayeeId,
                Effective = batch.Effective,
                Memo = "Batch Expense",
                Comments = this.Comments,
                Amount = this.Amount.GetValueOrDefault()
            };
        }

        public static ClientExpense FromTransaction(Expense expense)
        {
            var account = expense.Account as ClientAccount;
            var residency = account.Residency;
            var client = residency.Client;

            return new ClientExpense
            {
                Id = expense.Id,
                ReferenceNumber = expense.BatchReferenceNumber,
                ClientId = client.Id,
                ClientName = client.DisplayName,
                AccountId = expense.AccountId,
                AccountName = expense.Account.Name,
                ExpenseCategoryId = expense.ExpenseCategoryId.GetValueOrDefault(),
                ExpenseCategoryName = expense.ExpenseCategory.Name,
                Amount = expense.Amount,
                Comments = expense.Comments
            };
        }
    }


}