using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundsSearcher : ISearch<Fund>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public FundsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public ILogger Logger { get; set; }
        public IQueryable<Fund> All()
        {
            return context.Funds
                .Where(FilterByOrganization());
        }

        public IQueryable<Fund> GetById(int id)
        {
            return context.Funds
                .Where(FilterByOrganization())
                .Where(FilterById(id));
        }

        public IQueryable<Fund> GetBySearch(SearchPager searchPager)
        {
            return context.Funds
                .Where(FilterByOrganization())
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Fund, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();

            return fund => fund.OrganizationId == organization.Id;
        }

        private Expression<Func<Fund, bool>> FilterById(int id)
        {
            return fund => fund.Id == id;
        }

        private Expression<Func<Fund, bool>> FilterByIsActive(bool isActive)
        {
            return fund => fund.IsActive == isActive;
        }

        private Expression<Func<Fund, bool>> FilterBySearch(string search)
        {
            Expression<Func<Fund, bool>> noop =
                fund => true;

            Expression<Func<Fund, bool>> searchExpression =
                fund => fund.Name.ToLower().StartsWith(search.ToLower());

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }
}