using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Accounts;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class TransactionsSearcher : ISearch<Transaction>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public TransactionsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<Transaction> All()
        {
            return context.Transactions
                .Where(FilterByOrganization());
        }

        public IQueryable<Transaction> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<Transaction> GetBySearch(SearchPager searchPager)
        {
            var pager = searchPager as TransactionsSearchPager;

            if (pager == null)
                return All()
                    .Where(FilterBySearch(searchPager.Search));

            return All()
                .Where(FilterBySearch(pager.Search))
                .Where(FilterByDates(pager.FromDate, pager.ToDate));
        }

        private Expression<Func<Transaction, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return transaction => transaction.OrganizationId == organization.Id;
        }

        private Expression<Func<Transaction, bool>> FilterById(int id)
        {
            return transaction => transaction.Id == id;
        }

        private Expression<Func<Transaction, bool>> FilterBySearch(string search)
        {
            var number = search.ToNullableInt32();
            var money = search.ToNullableDecimal();

            Expression<Func<Transaction, bool>> nosearch = transaction => true;

            Expression<Func<Transaction, bool>> numberSearch =
                number.HasValue ? transaction => transaction.Id == number.Value
                : nosearch;

            Expression<Func<Transaction, bool>> moneySearch =
                money.HasValue ? transaction => transaction.Amount == money.Value
                : nosearch;

            if (string.IsNullOrWhiteSpace(search))
                return nosearch;

            if (number.HasValue && money.HasValue)
                return numberSearch.Or(moneySearch);

            if (number.HasValue)
                return numberSearch;

            return moneySearch;
        }

        private Expression<Func<Transaction, bool>> FilterByDates(DateTime? fromDate, DateTime? toDate)
        {
            Expression<Func<Transaction, bool>> nosearch = transaction => true;
            Expression<Func<Transaction, bool>> fromDateSearch = transaction => transaction.Created >= fromDate;
            Expression<Func<Transaction, bool>> toDateSearch = transaction => transaction.Created <= toDate;

            if (!fromDate.HasValue && !toDate.HasValue)
                return nosearch;

            if (fromDate.HasValue && !toDate.HasValue)
                return fromDateSearch;

            if (!fromDate.HasValue)
                return toDateSearch;

            return fromDateSearch.And(toDateSearch);
        }
    }

    public class SearchExpenses : ISearch<Expense>
    {
        private readonly ISearch<Transaction> transactions;

        public SearchExpenses(ISearch<Transaction> transactions)
        {
            this.transactions = transactions;
        }

        public IQueryable<Expense> All()
        {
            return transactions.All()
                .OfType<Expense>();
        }

        public IQueryable<Expense> GetById(int id)
        {
            return transactions.GetById(id)
                .OfType<Expense>();
        }

        public IQueryable<Expense> GetBySearch(SearchPager searchPager)
        {
            return transactions.GetBySearch(searchPager)
                .OfType<Expense>()
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Expense, bool>> FilterBySearch(string search)
        {
            Expression<Func<Expense, bool>> searchExpression =
                transaction => transaction.Payee.Name.StartsWith(search);

            return string.IsNullOrWhiteSpace(search)
                || search.ToNullableInt32().HasValue
                || search.ToNullableDecimal().HasValue
                ? expense => true : searchExpression;

        }

    }

    public class SearchReceipts : ISearch<Receipt>
    {
        private readonly ISearch<Transaction> transactions;

        public SearchReceipts(ISearch<Transaction> transactions)
        {
            this.transactions = transactions;
        }

        public IQueryable<Receipt> All()
        {
            return transactions.All()
                .OfType<Receipt>();
        }

        public IQueryable<Receipt> GetById(int id)
        {
            return transactions.GetById(id)
                .OfType<Receipt>();
        }

        public IQueryable<Receipt> GetBySearch(SearchPager searchPager)
        {
            return transactions.GetBySearch(searchPager)
                .OfType<Receipt>()
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Receipt, bool>> FilterBySearch(string search)
        {

            Expression<Func<Receipt, bool>> searchExpression =
                 transaction => transaction.ReceivedFrom.StartsWith(search)
                 || transaction.ReceiptNumber.StartsWith(search);

            return string.IsNullOrWhiteSpace(search)
                        || search.ToNullableInt32().HasValue
                        || search.ToNullableDecimal().HasValue
                ? receipt => true : searchExpression;
        }
    }

    public class TransactionBatchSearcher: ISearch<TransactionBatch>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public TransactionBatchSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<TransactionBatch> All()
        {
            return context.Batches
                .Where(FilterByOrganization());
        }

        public IQueryable<TransactionBatch> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<TransactionBatch> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<TransactionBatch, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return batch => batch.OrganizationId == organization.Id;
        }

        private Expression<Func<TransactionBatch, bool>> FilterById(int id)
        {
            return batch => batch.Id == id;
        }

        private Expression<Func<TransactionBatch, bool>> FilterByIsActive(bool isActive)
        {
            Expression<Func<TransactionBatch, bool>> activeExpression = batch => batch.Committed != isActive;

            return activeExpression;
        }

        private Expression<Func<TransactionBatch, bool>> FilterBySearch(string search)
        {
            Expression<Func<TransactionBatch, bool>> noop = batch => true;

            Expression<Func<TransactionBatch, bool>> searchExpression =
                batch => batch.BatchReferenceNumber.Equals(search)
                         || batch.Transactions.Any(t => t.BatchReferenceNumber.Equals(search));

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }

    public class SearchExpenseBatches : ISearch<ExpenseBatch>
    {
        private readonly ISearch<TransactionBatch> batches;

        public SearchExpenseBatches(ISearch<TransactionBatch>  batches)
        {
            this.batches = batches;
        }

        public IQueryable<ExpenseBatch> All()
        {
            return batches.All()
                .OfType<ExpenseBatch>();
        }

        public IQueryable<ExpenseBatch> GetById(int id)
        {
            return batches.GetById(id)
                .OfType<ExpenseBatch>();
        }

        public IQueryable<ExpenseBatch> GetBySearch(SearchPager searchPager)
        {
            return All()
                .OfType<ExpenseBatch>()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<ExpenseBatch, bool>> FilterByIsActive(bool isActive)
        {
            Expression<Func<ExpenseBatch, bool>> activeExpression = batch => batch.Committed != isActive;

            return activeExpression;
        }

        private Expression<Func<ExpenseBatch, bool>> FilterBySearch(string search)
        {
            Expression<Func<ExpenseBatch, bool>> noop = batch => true;

            Expression<Func<ExpenseBatch, bool>> referenceSearch =
                batch => batch.BatchReferenceNumber.Equals(search)
                         || batch.Transactions.Any(t => t.BatchReferenceNumber.Equals(search));

            Expression<Func<ExpenseBatch, bool>> payeeSearch =
                batch => batch.Transactions.OfType<Expense>().Any(r => r.Payee.Name.StartsWith(search));

            Expression<Func<ExpenseBatch, bool>> accountNameSearch =
                batch => batch.Transactions.OfType<Expense>().Any(r => r.Account is SubsidiaryAccount 
                                                                       && (r.Account as SubsidiaryAccount).Name.StartsWith(search));

            Expression<Func<ExpenseBatch, bool>> clientLastNameSearch =
                batch => batch.Transactions.OfType<Expense>().Any(r => r.Account is ClientAccount 
                && (r.Account as ClientAccount).Residency.Client.LastName.StartsWith(search));

            Expression<Func<ExpenseBatch, bool>> clientFirstNameSearch =
                batch => batch.Transactions.OfType<Expense>().Any(r => r.Account is ClientAccount
                && (r.Account as ClientAccount).Residency.Client.FirstName.StartsWith(search));
            
            Expression<Func<ExpenseBatch, bool>> clobber =
                referenceSearch
                .Or(payeeSearch)
                .Or(accountNameSearch)
                .Or(clientLastNameSearch).Or(clientFirstNameSearch);

            return string.IsNullOrWhiteSpace(search) ? noop : clobber;

        }
    }

    public class SearchReceiptBatches : ISearch<ReceiptBatch>
    {
        private readonly ISearch<TransactionBatch> batches;

        public SearchReceiptBatches(ISearch<TransactionBatch> batches)
        {
            this.batches = batches;
        }

        public IQueryable<ReceiptBatch> All()
        {
            return batches.All()
                .OfType<ReceiptBatch>();
        }

        public IQueryable<ReceiptBatch> GetById(int id)
        {
            return batches.GetById(id)
                .OfType<ReceiptBatch>();
        }

        public IQueryable<ReceiptBatch> GetBySearch(SearchPager searchPager)
        {
            return All()
               .OfType<ReceiptBatch>()
               .Where(FilterByIsActive(searchPager.IsActive))
               .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<ReceiptBatch, bool>> FilterByIsActive(bool isActive)
        {
            Expression<Func<ReceiptBatch, bool>> activeExpression = batch => batch.Committed != isActive;

            return activeExpression;
        }

        private Expression<Func<ReceiptBatch, bool>> FilterBySearch(string search)
        {
            Expression<Func<ReceiptBatch, bool>> noop = batch => true;

            Expression<Func<ReceiptBatch, bool>> referenceSearch =
                batch => batch.BatchReferenceNumber.Equals(search)
                         || batch.Transactions.Any(t => t.BatchReferenceNumber.Equals(search));

            Expression<Func<ReceiptBatch, bool>> receivedFromSearch =
                batch => batch.Transactions.OfType<Receipt>().Any(r => r.ReceivedFrom.StartsWith(search));

            Expression<Func<ReceiptBatch, bool>> clientLastNameSearch =
                batch => batch.Transactions.OfType<Receipt>().Any(r => r.Account is ClientAccount 
                && (r.Account as ClientAccount).Residency.Client.LastName.StartsWith(search));

            Expression<Func<ReceiptBatch, bool>> clientFirstNameSearch =
                batch => batch.Transactions.OfType<Receipt>().Any(r => r.Account is ClientAccount 
                && (r.Account as ClientAccount).Residency.Client.FirstName.StartsWith(search));

            Expression<Func<ReceiptBatch, bool>> accountNameSearch =
                batch => batch.Transactions.OfType<Receipt>().Any(r => r.Account is SubsidiaryAccount 
                && (r.Account as SubsidiaryAccount).Name.StartsWith(search));

            Expression<Func<ReceiptBatch, bool>> clobber =
                referenceSearch
                .Or(receivedFromSearch)
                .Or(accountNameSearch)
                .Or(clientLastNameSearch).Or(clientFirstNameSearch);

            return string.IsNullOrWhiteSpace(search) ? noop : clobber;
        }
    }

    public class SearchTransferBatches : ISearch<TransferBatch>
    {
        private readonly ISearch<TransactionBatch> batches;

        public SearchTransferBatches(ISearch<TransactionBatch> batches)
        {
            this.batches = batches;
        }

        public IQueryable<TransferBatch> All()
        {
            return batches.All()
                .OfType<TransferBatch>();
        }

        public IQueryable<TransferBatch> GetById(int id)
        {
            return batches.GetById(id)
                .OfType<TransferBatch>();
        }

        public IQueryable<TransferBatch> GetBySearch(SearchPager searchPager)
        {
            throw new NotImplementedException();
        }
    }
}