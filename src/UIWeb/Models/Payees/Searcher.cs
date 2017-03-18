using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeesSearcher : ISearch<Payee>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public PayeesSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<Payee> All()
        {
            return context.Payees
                .Where(FilterByOrganization());
        }

        public IQueryable<Payee> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<Payee> GetBySearch(SearchPager searchPager)
        {
            var pager = searchPager as PayeesSearchPager;

            if (pager != null)
            {
                return All()
                    .Where(FilterByIsActive(pager.IsActive))
                    .Where(FilterByType(pager.Type))
                    .Where(FilterBySearch(pager.Search));
            }

            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Payee, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return payee => payee.OrganizationId == organization.Id;

        }

        private Expression<Func<Payee, bool>> FilterById(int id)
        {
            return payee => payee.Id == id;
        }

        private Expression<Func<Payee, bool>> FilterByIsActive(bool isActive)
        {
            return payee => payee.IsActive == isActive;
        }

        private Expression<Func<Payee, bool>> FilterByType(string type)
        {

            Expression<Func<Payee, bool>> noop =
                payee => true;

            Expression<Func<Payee, bool>> expression =
                payee => payee.Types.Any(t => type.Contains(t.Name));

            return string.IsNullOrWhiteSpace(type) ? noop : expression;
        }

        private Expression<Func<Payee, bool>> FilterBySearch(string search)
        {
            Expression<Func<Payee, bool>> noop =
                payee => true;

            Expression<Func<Payee, bool>> searchExpression =
                payee => payee.Name.ToLower().StartsWith(search.ToLower());

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }
}