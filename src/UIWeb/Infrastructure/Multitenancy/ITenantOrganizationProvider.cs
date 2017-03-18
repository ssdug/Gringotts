using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.Ajax.Utilities;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy
{
    public interface ITenantOrganizationProvider
    {
        Organization GetTenantOrganization();
        IList<Organization> GetAllTenantOrganizationsForUser();
    }

    public class TenantOrganizationProvider : ITenantOrganizationProvider
    {
        public ILogger Logger { get; set; }

        private readonly ApplicationDbContext dbContext;
        private readonly ITenantKeyManager tenantKeyManager;
        private readonly IPrincipalProvider principalProvider;

        private Organization currentOrganization;

        public TenantOrganizationProvider(ApplicationDbContext dbContext, 
            ITenantKeyManager tenantKeyManager, 
            IPrincipalProvider principalProvider)
        {
            this.dbContext = dbContext;
            this.tenantKeyManager = tenantKeyManager;
            this.principalProvider = principalProvider;
        }

        public Organization GetTenantOrganization()
        {
            Logger.Trace("GetTenantOrganization");

            if (currentOrganization != null)
            {
                Logger.Trace("GetTenantOrganization:Prefetched");
                return currentOrganization;
            }

            var principal = principalProvider.GetCurrent();
            var organizations = GetAllTenantOrganizationsForUser();
            var tenantKey = tenantKeyManager.GetTenantKey();

            Logger.Trace("GetTenantOrganization:SingleOrganization");
            if (organizations.Count() == 1 && !organizations.Any(o => o.Children.Any()))
                return currentOrganization = organizations.First();

            Logger.Trace("GetTenantOrganization:TenantKey");
            if (!string.IsNullOrEmpty(tenantKey))
                return currentOrganization = organizations.FirstOrDefault(o => o.Abbreviation.Equals(tenantKey));

            Logger.Warn("unable to locate tenant organization for {0}".FormatWith(principal.Identity.Name));

            return null;
        }

        public IList<Organization> GetAllTenantOrganizationsForUser()
        {
            Logger.Trace("GetAllTenantOrganizationsForUser");

            //TODO:pulling back all organizations is prolly not the best
            //approach here, but we have a limited number or orgs
            //so this works for now.
            var principal = principalProvider.GetCurrent();
            var organizations = dbContext.Organizations
                .Include(o => o.Children)
                .ToArray();

            var principalsOrganizations = organizations.Where(o => principal.IsInRole(o.GroupName))
                .Flatten(o => o.Children)
                .DistinctBy(o => o.Id)
                .ToArray();

            if (!principalsOrganizations.Any())
            {
                Logger.Warn("user {0} does not belong to any organizations".FormatWith(principal.Identity.Name));
            }

            return principalsOrganizations;
        } 
    }
}