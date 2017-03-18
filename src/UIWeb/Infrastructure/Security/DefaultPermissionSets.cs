using System;
using System.Collections.Generic;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Security
{
    public class DefaultPermissionSets : IPermissionSetProvider
    {
        public IDictionary<Type, IDictionary<Permissions.Action, IEnumerable<string>>> GetPermissionSets()
        {
            return new Dictionary<Type, IDictionary<Permissions.Action, IEnumerable<string>>>
            {
                { typeof (Organization), new Dictionary<Permissions.Action, IEnumerable<string>> {
                    {Permissions.Action.Create, new[] { ApplicationRoles.SystemAdministrator }},
                    { Permissions.Action.Read, new[] { ApplicationRoles.SystemAdministrator, ApplicationRoles.Developer }},
                    { Permissions.Action.Update, new[] { ApplicationRoles.SystemAdministrator }},
                    { Permissions.Action.Delete, new[] { ApplicationRoles.SystemAdministrator }},}
                },

            };
        }
    }
}