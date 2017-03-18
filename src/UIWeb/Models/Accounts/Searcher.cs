using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AccountsSearcher : ISearch<Account>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;
        public ILogger Logger { get; set; }

        public AccountsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public virtual IQueryable<Account> All()
        {
            return context.Accounts
                .Where(FilterByOrganization());
        }

        public virtual IQueryable<Account> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<Account> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Account, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();

            return account => account.OrganizationId == organization.Id;
        }

        private Expression<Func<Account, bool>> FilterById(int id)
        {
            return account => account.Id == id;
        }

        private Expression<Func<Account, bool>> FilterByIsActive(bool isActive)
        {
            Logger.Trace("FilterByIsActive::{0}", isActive);

            return account => account.IsActive == isActive;
        }

        private Expression<Func<Account, bool>> FilterBySearch(string search)
        {
            Logger.Trace("FilterBySearch::{0}", search);

            Expression<Func<Account, bool>> noSearchExpression =
                account => true;

            Expression<Func<Account, bool>> subsidiaryAccountSearch =
                account => account.Name.Contains(search)
                           || account.BankNumber.StartsWith(search);


            return string.IsNullOrWhiteSpace(search) 
                ? noSearchExpression : subsidiaryAccountSearch;
        }
    }

    public class SubsidiaryAccountsSearcher : ISearch<SubsidiaryAccount>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Account> accounts;

        public SubsidiaryAccountsSearcher(ISearch<Account> accounts)
        {
            this.accounts = accounts;
        }

        public IQueryable<SubsidiaryAccount> All()
        {
            return accounts.All().OfType<SubsidiaryAccount>();
        }

        public IQueryable<SubsidiaryAccount> GetById(int id)
        {
            return accounts.GetById(id).OfType<SubsidiaryAccount>();
        }

        public IQueryable<SubsidiaryAccount> GetBySearch(SearchPager searchPager)
        {
            return accounts.GetBySearch(searchPager).OfType<SubsidiaryAccount>();
        }
    }

    public class ClientAccountSearcher : ISearch<ClientAccount>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Account> accounts;

        public ClientAccountSearcher(ISearch<Account> accounts)
        {
            this.accounts = accounts;
        }

        public IQueryable<ClientAccount> All()
        {
            return accounts.All()
                .OfType<ClientAccount>();
        }

        public IQueryable<ClientAccount> GetById(int id)
        {
            return accounts.GetById(id)
                .OfType<ClientAccount>();
        }

        public IQueryable<ClientAccount> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<ClientAccount, bool>> FilterByIsActive(bool isActive)
        {
            Logger.Trace("FilterByIsActive::{0}", isActive);

            return account => account.IsActive == isActive;
        }

        private Expression<Func<ClientAccount, bool>> FilterBySearch(string search)
        {
            Logger.Trace("FilterBySearch::{0}", search);

            Expression<Func<ClientAccount, bool>> noSearchExpression =
                client => true;

            Expression<Func<ClientAccount, bool>> searchExpression =
                clientAccount => clientAccount.Residency.Client.DisplayName.StartsWith(search)
                      || clientAccount.Residency.Client.FirstName.StartsWith(search)
                      || clientAccount.Residency.Client.Identifiers.Any(i => i.Value
                          .Equals(search))
                      || clientAccount.BankNumber.Equals(search);

            return string.IsNullOrWhiteSpace(search) ? noSearchExpression : searchExpression;
        }
    }
}