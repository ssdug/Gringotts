using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using Wiz.Gringotts.UIWeb.Models.Users;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class PrincipalExtensions
    {
        public static User BuildNewUser(this IPrincipal user)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var p = UserPrincipal.FindByIdentity(context, user.Identity.Name);
                return new User
                {
                    UserName = user.Identity.Name,
                    FirstName = p.GivenName,
                    LastName = p.Surname,
                    PhoneNumber = p.VoiceTelephoneNumber,
                    EmailAddress = p.EmailAddress
                };
            }
        }

        public static IDictionary<string, string> GetProperties(this UserPrincipal user)
        {
            var de = user.GetUnderlyingObject() as DirectoryEntry;
            if (de != null)
            {
                return de.Properties.Cast<PropertyValueCollection>()
                    .OrderBy(p => p.PropertyName)
                    .ToDictionary(property => property.PropertyName, 
                            property => property.Value.ToString());
            }

            return new Dictionary<string, string>();
        }
    }
}