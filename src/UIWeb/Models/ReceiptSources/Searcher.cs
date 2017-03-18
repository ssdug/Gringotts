using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourcesSearcher : ISearch<ReceiptSource>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public ReceiptSourcesSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<ReceiptSource> All()
        {
            return context.ReceiptSources
                .Where(FilterByOrganization());
        }

        public IQueryable<ReceiptSource> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<ReceiptSource> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<ReceiptSource, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return category => category.OrganizationId == organization.Id;
        }

        private Expression<Func<ReceiptSource, bool>> FilterById(int id)
        {
            return category => category.Id == id;
        }

        private Expression<Func<ReceiptSource, bool>> FilterByIsActive(bool isActive)
        {
            return category => category.IsActive == isActive;
        }

        private Expression<Func<ReceiptSource, bool>> FilterBySearch(string search)
        {
            Expression<Func<ReceiptSource, bool>> noop =
                payee => true;

            Expression<Func<ReceiptSource, bool>> searchExpression =
                category => category.Name.ToLower().StartsWith(search.ToLower());

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }
}