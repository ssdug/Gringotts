using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientsSearcher : ISearch<Client>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;
        public ILogger Logger { get; set; }

        public ClientsSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<Client> All()
        {
            return context.Clients
                .Where(FilterByOrganization());
        }

        public IQueryable<Client> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<Client> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterByIsActive(searchPager.IsActive))
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Client, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return client => client.Residencies.Any(r => r.OrganizationId == organization.Id);
        }

        private Expression<Func<Client, bool>> FilterById(int id)
        {
            return client => client.Id == id;
        }

        private Expression<Func<Client, bool>> FilterByIsActive(bool isActive)
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return client => client.Residencies.Any(r => r.OrganizationId == organization.Id && r.IsActive == isActive);
        }

        private Expression<Func<Client, bool>> FilterBySearch(string search)
        {
            Expression<Func<Client, bool>> noSearchExpression =
                client => true;

            Expression<Func<Client, bool>> searchExpression =
                client => client.DisplayName.StartsWith(search)
                      || client.FirstName.StartsWith(search)
                      || client.Identifiers.Any(i => i.Value
                          .Equals(search));

            return string.IsNullOrWhiteSpace(search) ? noSearchExpression : searchExpression;
        }
    }
}