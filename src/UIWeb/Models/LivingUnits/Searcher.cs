using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.LivingUnits
{
    public class LivingUnitsSearcher : ISearch<LivingUnit>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public LivingUnitsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<LivingUnit> All()
        {
            return context.LivingUnits
                .Where(FilterByOrganization());
        }

        public IQueryable<LivingUnit> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<LivingUnit> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<LivingUnit, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return unit => unit.OrganizationId == organization.Id;
        }

        private Expression<Func<LivingUnit, bool>> FilterById(int id)
        {
            return unit => unit.Id == id;
        }

        private Expression<Func<LivingUnit, bool>> FilterByIsActive(bool isActive)
        {
            return unit => unit.IsActive == isActive;
        }

        private Expression<Func<LivingUnit, bool>> FilterBySearch(string search)
        {
            Expression<Func<LivingUnit, bool>> noop =
                livingUnit => true;

            Expression<Func<LivingUnit, bool>> searchExpression =
                livingUnit => livingUnit.Name.ToLower().StartsWith(search.ToLower());

            return string.IsNullOrWhiteSpace(search) ? noop : searchExpression;
        }
    }
}