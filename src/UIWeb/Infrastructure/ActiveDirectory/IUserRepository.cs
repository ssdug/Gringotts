using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Wiz.Gringotts.UIWeb.Models.Users;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{
    public interface IUserRepository
    {
        User CurrentUser();
        User FindByUser(IPrincipal principal);
        User FindByUser(string samAccountName);
        IEnumerable<User> FindByOrganization(string distinguishedName);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ILdapProvider ldapProvider;
        private readonly IPrincipalProvider principalProvider;

        public UserRepository(ILdapProvider ldapProvider, IPrincipalProvider principalProvider)
        {
            this.ldapProvider = ldapProvider;
            this.principalProvider = principalProvider;
        }

        public User CurrentUser()
        {
            return FindByUser(principalProvider.GetCurrent());
        }

        public User FindByUser(IPrincipal principal)
        {
            User user = null;

            ldapProvider.WithUserPrincipal(principal.Identity.Name,
                userPrincipal => user = MapToUser(userPrincipal));

            return user;
        }

        public User FindByUser(string samAccountName)
        {
            User user = null;

            ldapProvider.WithUserPrincipal(samAccountName,
                userPrincipal => user = MapToUser(userPrincipal));

            return user;
        }

        public IEnumerable<User> FindByOrganization(string distinguishedName)
        {
            ICollection<User> users = new List<User>();

            ldapProvider.WithGroupPrincipals(distinguishedName,
                userPrincipals => userPrincipals.ForEach(p => users.Add(MapToUser(p))));

            return users;
        }

        private User MapToUser(UserPrincipal user)
        {
            if (user == null)
                return null;

            return new User
            {
                UserName = user.SamAccountName,
                FirstName = user.GivenName,
                LastName = user.Surname,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.VoiceTelephoneNumber
            };
        }
    }
}
