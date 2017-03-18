using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.ExpenseCategories
{
    public class ExpenseCategoriesSearcher : ISearch<ExpenseCategory>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public ExpenseCategoriesSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<ExpenseCategory> All()
        {
            return context.ExpenseCategories
                .Where(FilterByOrganization());
        }

        public IQueryable<ExpenseCategory> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<ExpenseCategory> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<ExpenseCategory, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return category => category.OrganizationId == organization.Id;
        }

        private Expression<Func<ExpenseCategory, bool>> FilterById(int id)
        {
            return category => category.Id == id;
        }

        private Expression<Func<ExpenseCategory, bool>> FilterByIsActive(bool isActive)
        {
            return category => category.IsActive == isActive;
        }

        private Expression<Func<ExpenseCategory, bool>> FilterBySearch(string search)
        {
            Expression<Func<ExpenseCategory, bool>> noop =
                payee => true;

            Expression<Func<ExpenseCategory, bool>> searchExpression =
                category => category.Name.ToLower().StartsWith(search.ToLower());

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }
}