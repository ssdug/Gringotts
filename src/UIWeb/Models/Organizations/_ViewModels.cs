using System.Collections.Generic;
using System.Linq;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models.Users;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class OrganizationDetails
    {
        public OrganizationDetails()
        {
            Users = Enumerable.Empty<User>();
        }
        public Organization Organization { get; set; }
        public User FiscalContact { get; set; }
        public User ITContact { get; set; }
        public IEnumerable<User> Users { get; set; }
    }

    public class OrganizationTenantList
    {
        public string ReturnUrl { get; private set; }
        public IEnumerable<Organization> TenantOrganizations { get; private set; }

        public OrganizationTenantList(string returnUrl, IEnumerable<Organization> tenantOrganizations)
        {
            this.ReturnUrl = returnUrl;
            this.TenantOrganizations = tenantOrganizations;
        }
    }

    public class TenantBreadcrumb
    {
        public IEnumerable<Organization> AvailableTenantOrganizations { get; set; }
        public Organization CurrentTenantOrganization { get; set; }

        public bool IsAvailable(Organization org)
        {
            return this.AvailableTenantOrganizations.Contains(org);
        }

        public IEnumerable<Organization> ToHeirarchy()
        {
            return this.CurrentTenantOrganization.ToHeirarchy();
        }

        public bool IsCurrentTenant(Organization org)
        {
            return org.Id == CurrentTenantOrganization.Id;
        }

        public bool HasParent(Organization org)
        {
            return org.Parent != null;
        }

        public bool HasChildren(Organization org)
        {
            return org.Children != null
                   && org.Children.Any();
        }

        public bool HasAvailableChildren(Organization org)
        {
            return org.Children != null
                   && GetAvailableChildren(org)
                       .Any();
        }

        public IEnumerable<Organization> GetAvailableChildren(Organization org)
        {
            return org.Children == null
                ? Enumerable.Empty<Organization>()
                : org.Children
                    .Where(c => AvailableTenantOrganizations.Contains(c));
        }

        public bool HasAvailableSiblings(Organization org)
        {
            return org.Parent != null
                   && GetAvailableSiblings(org)
                       .Any();
        }

        public IEnumerable<Organization> GetAvailableSiblings(Organization org)
        {
            return org.Parent == null
                ? Enumerable.Empty<Organization>()
                : org.Parent.Children
                    .Where(c => c.Id != org.Id)
                    .Where(c => AvailableTenantOrganizations.Contains(c));
        }

        public bool HasAvailableSiblingChildren(Organization org)
        {
            return org.Parent != null
                   && org.Parent.Children != null
                   && org.Parent.Children.Any(s =>
                   {
                       return s.Children != null
                              && s.Children.Any(c => IsAvailable(c) && c.Parent.Id != CurrentTenantOrganization.Parent.Id);
                   });
        }

        public IEnumerable<Organization> GetAvailableSiblingChildren(Organization org)
        {
            return org.Parent == null || org.Parent.Children == null
                ? Enumerable.Empty<Organization>()
                : org.Parent.Children
                    .SelectMany(s => s.Children)
                    .Where(IsAvailable);
        }
    }
}