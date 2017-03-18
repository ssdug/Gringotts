using System.Collections.Generic;
using System.Linq;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public abstract partial class BatchReceiptEditorForm
    {
        public void UpdateBatch(ReceiptBatch batch, ApplicationDbContext context)
        {
            batch.TransactionSubTypeId = ReceiptTypeId;
            batch.ExpectedAmount = ExpectedAmount.GetValueOrDefault();
            batch.Effective = EffectiveDate.Date;
            UpdateTransactions(batch, context);
        }

        protected abstract void UpdateTransactions(ReceiptBatch batch, ApplicationDbContext context);
    }

    public partial class SubsidiaryBatchReceiptEditorForm
    {
        protected override void UpdateTransactions(ReceiptBatch batch, ApplicationDbContext context)
        {
            var accountLookup = GetAccountLookup(batch, context);

            //remove deleted transactions
            batch.Transactions.OfType<Receipt>()
                .Where(receipt => this.Transactions.All(editor => editor.Id != receipt.Id))
                .ToArray()
                .ForEach(receipt =>
                {
                    var account = accountLookup[receipt.AccountId];
                    account.RemoveReceipt(receipt);
                    batch.Transactions.Remove(receipt);
                });

            //edit existing transactions
            batch.Transactions.OfType<Receipt>()
                .Where(receipt => this.Transactions.Any(editor => editor.Id == receipt.Id))
                .ToArray()
                .ForEach(receipt =>
                {
                    var account = accountLookup[receipt.AccountId];
                    var editor = this.Transactions.Single(e => e.Id == receipt.Id);
                    account.RemoveReceipt(receipt);
                    editor.Update(receipt, batch);
                    account.AddReceipt(receipt);
                });

            //add new transactions
            this.Transactions.Where(editor => !editor.Id.HasValue)
                .ForEach(editor =>
                {
                    var account = accountLookup[editor.AccountId];
                    var receipt = editor.BuildReceipt(batch, context);
                    account.AddReceipt(receipt);
                    batch.Transactions.Add(receipt);
                });
        }

        private IDictionary<int, SubsidiaryAccount> GetAccountLookup(ReceiptBatch batch, ApplicationDbContext context)
        {
            var ids = this.Transactions.Select(t => t.AccountId)
                     .Union(batch.Transactions.Select(t => t.AccountId))
                     .Distinct()
                     .ToArray();

            return context.Accounts.Where(a => ids.Contains(a.Id))
                .OfType<SubsidiaryAccount>()
                .ToDictionary(a => a.Id);
        }
    }

    public partial class SubsidiaryReceipt
    {
        public void Update(Receipt receipt, ReceiptBatch batch)
        {
            receipt.AccountId = this.AccountId;
            receipt.ReceiptSourceId = ReceiptSourceId;
            receipt.TransactionSubTypeId = batch.TransactionSubTypeId.Value;
            receipt.Effective = batch.Effective;
            receipt.ReceiptNumber = ReceiptNumber;
            receipt.ReceivedFor = ReceivedFor;
            receipt.ReceivedFrom = ReceivedFrom;
            receipt.Comments = this.Comments;
            receipt.Amount = this.Amount.GetValueOrDefault();
        }
    }

    public partial class ClientBatchReceiptEditorForm
    {
        protected override void UpdateTransactions(ReceiptBatch batch, ApplicationDbContext context)
        {
            var accountLookup = GetAccountLookup(batch, context);

            //remove deleted transactions
            batch.Transactions.OfType<Receipt>()
                .Where(receipt => this.Transactions.All(editor => editor.Id != receipt.Id))
                .ToArray()
                .ForEach(receipt =>
                {
                    var account = accountLookup[receipt.AccountId];
                    account.RemoveReceipt(receipt);
                    batch.Transactions.Remove(receipt);
                });

            //edit existing transactions
            batch.Transactions.OfType<Receipt>()
                .Where(receipt => this.Transactions.Any(editor => editor.Id == receipt.Id))
                .ToArray()
                .ForEach(receipt =>
                {
                    var account = accountLookup[receipt.AccountId];
                    var editor = this.Transactions.Single(e => e.Id == receipt.Id);
                    account.RemoveReceipt(receipt);
                    editor.Update(receipt, batch);
                    account.AddReceipt(receipt);
                });

            //add new transactions
            this.Transactions.Where(editor => !editor.Id.HasValue)
                .ForEach(editor =>
                {
                    var account = accountLookup[editor.AccountId];
                    var receipt = editor.BuildReceipt(batch, context);
                    account.AddReceipt(receipt);
                    batch.Transactions.Add(receipt);
                });
        }

        private IDictionary<int, ClientAccount> GetAccountLookup(ReceiptBatch batch, ApplicationDbContext context)
        {
            var ids = this.Transactions.Select(t => t.AccountId)
                     .Union(batch.Transactions.Select(t => t.AccountId))
                     .Distinct()
                     .ToArray();

            return context.Accounts.Where(a => ids.Contains(a.Id))
                .OfType<ClientAccount>()
                .ToDictionary(a => a.Id);
        }
    }

    public partial class ClientReceipt
    {
        public void Update(Receipt receipt, ReceiptBatch batch)
        {
            receipt.AccountId = this.AccountId;
            receipt.ReceiptSourceId = ReceiptSourceId;
            receipt.TransactionSubTypeId = batch.TransactionSubTypeId.Value;
            receipt.Effective = batch.Effective;
            receipt.ReceiptNumber = ReceiptNumber;
            receipt.ReceivedFor = ReceivedFor;
            receipt.ReceivedFrom = ReceivedFrom;
            receipt.Comments = this.Comments;
            receipt.Amount = this.Amount.GetValueOrDefault();
        }
    }

    public abstract partial class BatchExpenseEditorForm
    {
        public void UpdateBatch(ExpenseBatch batch, ApplicationDbContext context)
        {
            batch.TransactionSubTypeId = this.ExpenseTypeId;
            batch.PayeeId = this.PayeeId;
            batch.ExpectedAmount = this.ExpectedAmount.GetValueOrDefault();
            batch.Effective = this.EffectiveDate.Date;
            batch.Memo = this.Memo;

            UpdateTransactions(batch, context);
        }

        protected abstract void UpdateTransactions(ExpenseBatch batch, ApplicationDbContext context);
    }

    public partial class SubsidiaryBatchExpenseEditorForm
    {
        protected override void UpdateTransactions(ExpenseBatch batch, ApplicationDbContext context)
        {
            var accountLookup = GetAccountLookup(batch, context);

            //remove deleted transactions
            batch.Transactions.OfType<Expense>()
                .Where(expense => this.Transactions.All(editor => editor.Id != expense.Id))
                .ToArray()
                .ForEach(expense =>
                {
                    var account = accountLookup[expense.AccountId];
                    account.RemoveExpense(expense);
                    batch.Transactions.Remove(expense);
                });

            //edit existing transactions
            batch.Transactions.OfType<Expense>()
                .Where(expense => this.Transactions.Any(editor => editor.Id == expense.Id))
                .ToArray()
                .ForEach(expense =>
                {
                    var account = accountLookup[expense.AccountId];
                    var editor = this.Transactions.Single(e => e.Id == expense.Id);
                    account.RemoveExpense(expense);
                    editor.Update(expense, batch);
                    account.AddExpense(expense);
                });

            //add new transactions
            this.Transactions.Where(editor => !editor.Id.HasValue)
                .ForEach(editor =>
                {
                    var account = accountLookup[editor.AccountId];
                    var expense = editor.BuildExpense(batch, context);
                    account.AddExpense(expense);
                    batch.Transactions.Add(expense);
                });
        }

        private IDictionary<int, SubsidiaryAccount> GetAccountLookup(ExpenseBatch batch, ApplicationDbContext context)
        {
            var ids = this.Transactions.Select(t => t.AccountId)
                     .Union(batch.Transactions.Select(t => t.AccountId))
                     .Distinct()
                     .ToArray();

            return context.Accounts.Where(a => ids.Contains(a.Id))
                .OfType<SubsidiaryAccount>()
                .ToDictionary(a => a.Id);
        }
    }

    public partial class SubsidiaryExpense
    {
        public void Update(Expense expense, ExpenseBatch batch)
        {
            expense.AccountId = this.AccountId;
            expense.ExpenseCategoryId = this.ExpenseCategoryId;
            expense.TransactionSubTypeId = batch.TransactionSubTypeId.Value;
            expense.PayeeId = batch.PayeeId;
            expense.Effective = batch.Effective;
            expense.Comments = this.Comments;
            expense.Amount = this.Amount.GetValueOrDefault();
        }
    }

    public partial class ClientBatchExpenseEditorForm
    {
        protected override void UpdateTransactions(ExpenseBatch batch, ApplicationDbContext context)
        {
            var accountLookup = GetAccountLookup(batch, context);

            //remove deleted transactions
            batch.Transactions.OfType<Expense>()
                .Where(expense => this.Transactions.All(editor => editor.Id != expense.Id))
                .ToArray()
                .ForEach(expense =>
                {
                    var account = accountLookup[expense.AccountId];
                    account.RemoveExpense(expense);
                    batch.Transactions.Remove(expense);
                });

            //edit existing transactions
            batch.Transactions.OfType<Expense>()
                .Where(expense => this.Transactions.Any(editor => editor.Id == expense.Id))
                .ToArray()
                .ForEach(expense =>
                {
                    var account = accountLookup[expense.AccountId];
                    var editor = this.Transactions.Single(e => e.Id == expense.Id);
                    account.RemoveExpense(expense);
                    editor.Update(expense, batch);
                    account.AddExpense(expense);
                });

            //add new transactions
            this.Transactions.Where(editor => !editor.Id.HasValue)
                .ForEach(editor =>
                {
                    var account = accountLookup[editor.AccountId];
                    var expense = editor.BuildExpense(batch, context);
                    account.AddExpense(expense);
                    batch.Transactions.Add(expense);
                });
        }

        private IDictionary<int, ClientAccount> GetAccountLookup(ExpenseBatch batch, ApplicationDbContext context)
        {
           var ids = this.Transactions.Select(t => t.AccountId)
                    .Union(batch.Transactions.Select(t => t.AccountId))
                    .Distinct()
                    .ToArray();

            return context.Accounts.Where(a => ids.Contains(a.Id))
                .OfType<ClientAccount>()
                .ToDictionary(a => a.Id);
        }
    }

    public partial class ClientExpense
    {
        public void Update(Expense expense, ExpenseBatch batch)
        {
            expense.AccountId = this.AccountId;
            expense.ExpenseCategoryId = this.ExpenseCategoryId;
            expense.TransactionSubTypeId = batch.TransactionSubTypeId.Value;
            expense.PayeeId = batch.PayeeId;
            expense.Effective = batch.Effective;
            expense.Comments = this.Comments;
            expense.Amount = this.Amount.GetValueOrDefault();
        }
    }
}