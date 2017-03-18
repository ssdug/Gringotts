using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrdersSearcher : ISearch<Order>
    {
        private readonly ApplicationDbContext context;
        private readonly ITenantOrganizationProvider tenantOrganizationProvider;
        public ILogger Logger { get; set; }

        public RestitutionOrdersSearcher(ApplicationDbContext context, ITenantOrganizationProvider tenantOrganizationProvider)
        {
            this.context = context;
            this.tenantOrganizationProvider = tenantOrganizationProvider;
        }

        public IQueryable<Order> All()
        {
            return context.Orders
                .Where(FilterByOrganization());
        }

        public IQueryable<Order> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }

        public IQueryable<Order> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Order, bool>> FilterBySearch(string search)
        {
            return order => true;
        }

        private Expression<Func<Order, bool>> FilterByOrganization()
        {
            var organization = tenantOrganizationProvider.GetTenantOrganization();
            return order => order.OrganizationId == organization.Id;
        }

        private Expression<Func<Order, bool>> FilterById(int id)
        {
            return order => order.Id == id;
        }
    }
}