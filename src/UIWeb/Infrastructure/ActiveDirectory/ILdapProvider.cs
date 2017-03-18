using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Autofac.Extras.NLog;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{

    public interface ILdapProvider
    {
        bool UserExists(string distinguishedName);
        bool GroupExists(string name);
        bool OrganizationExists(string distinguishedName);
        void WithUserPrincipal(string samAccountName, Action<UserPrincipal> action);
        void WithGroupPrincipals(string groupName, Action<IEnumerable<UserPrincipal>> action);
    }

    public class LdapProvider : ILdapProvider
    {
        public ILogger Logger { get; set; }

        private readonly IApplicationConfiguration config;

        public LdapProvider(IApplicationConfiguration config)
        {
            this.config = config;
        }

        public bool UserExists(string samAccountName)
        {
            Logger.Trace("UserExists::{0}", samAccountName);

            if (string.IsNullOrWhiteSpace(samAccountName))
            {
                throw new ArgumentException("SAM Account Name cannot be null","samAccountName");
            }

            var filter = "(&(objectClass=user)(sAMAccountName={0}))".FormatWith(samAccountName);
            return Exists(filter);
        }

        public bool GroupExists(string name)
        {
            Logger.Trace("GroupExists::{0}", name);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null", "name");
            }

            var filter = "(&(objectClass=group)(cn={0}))".FormatWith(name);
            return Exists(filter);
        }

        public bool OrganizationExists(string distinguishedName)
        {
            Logger.Trace("OrganizationExists::{0}", distinguishedName);

            if (string.IsNullOrWhiteSpace(distinguishedName))
            {
                throw new ArgumentException("Distinguished Name cannot be null", "distinguishedName");
            }

            var filter = "(&(objectClass=organizationalUnit)(distinguishedName={0}))".FormatWith(distinguishedName);
            return Exists(filter);
        }

        private bool Exists(string filter)
        {
            var exists = false;
            
            WithDirectorySearcher(ds =>
            {
                ds.ReferralChasing = ReferralChasingOption.All;
                ds.Filter = filter;
                var de = ds.FindOne();
                exists = de != null;
            });

            return exists;
        }

        private void WithDirectorySearcher(Action<DirectorySearcher> action)
        {
            WithBaseDirectoryEntry(e =>
            {
                using (var ds = new DirectorySearcher(e))
                {
                    action(ds);
                }
            });
        }

        private void WithBaseDirectoryEntry(Action<DirectoryEntry> action)
        {
            using (var e = new DirectoryEntry(config.LdapPath))
            {
                action(e);
            }
        }

        public void WithUserPrincipal(string samAccountName, Action<UserPrincipal> action)
        {
            Logger.Trace("WithUserPrincipal::{0}", samAccountName);

            if (string.IsNullOrWhiteSpace(samAccountName))
            {
                throw new ArgumentException("SAM Account Name cannot be null", "samAccountName");
            }

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(context, samAccountName);

                action(user);
            }
        }

        public void WithGroupPrincipals(string groupName, Action<IEnumerable<UserPrincipal>> action)
        {
            Logger.Trace("WithGroupPrincipals::{0}", groupName);

            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Distinguished Name cannot be null", "groupName");
            }

            using (var context = new PrincipalContext(ContextType.Domain, config.LdapPath))
            {
                var group = GroupPrincipal.FindByIdentity(context, groupName);

                //todo: remove this once all orgs have ad groups
                if (group == null)
                {
                    action(Enumerable.Empty<UserPrincipal>());
                    return;
                }
                var users = group.GetMembers(true).AsEnumerable()
                    .Cast<UserPrincipal>();

                action(users);
            }
        }
    }
}
