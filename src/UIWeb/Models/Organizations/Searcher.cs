using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class OrganizationsSearcher : ISearch<Organization>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;

        public OrganizationsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<Organization> All()
        {
            return context.Organizations;
        }

        public IQueryable<Organization> GetById(int id)
        {
            return context.Organizations
                .Where(FilterById(id));
        }

        public IQueryable<Organization> GetBySearch(SearchPager searchPager)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<Organization, bool>> FilterById(int id)
        {
            return organization => organization.Id == id;
        }
    }
}