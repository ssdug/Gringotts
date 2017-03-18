using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac.Extras.NLog;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Security
{
    public class Permissions
    {
        public ILogger Logger { get; set; }

        public enum Action
        {
            Create,
            Read,
            Update,
            Delete
        }

        public Lazy<IDictionary<Type, IDictionary<Action, IEnumerable<string>>>> PermissionSets { get; set; }

        public Permissions(IPermissionSetProvider provider)
        {
            PermissionSets = new Lazy<IDictionary<Type, IDictionary<Action, IEnumerable<string>>>>(provider.GetPermissionSets);
        }

        public bool CanPerform(IPrincipal principal, Type resourceType, Action action)
        {
            Logger.Info("CanPerform::{0}-{1}-{2}",
                principal==null? "Anonymous" : principal.Identity.Name, resourceType.Name, action.ToString());

            var result = false;
            var roles = from permission in PermissionSets.Value
                        where permission.Key == resourceType
                        where permission.Value.ContainsKey(action)
                        from items in permission.Value[action]
                        select items;

            result = (principal != null && roles.Any(principal.IsInRole));

            if (result == false)
            {
                Logger.Warn("CanPerform::{0}-{1}-{2} - Denied",
                    principal == null ? "Anonymous" : principal.Identity.Name, resourceType.Name, action.ToString());                
            }

            return result;
        }
    }

    public interface IPermissionSetProvider
    {
        IDictionary<Type, IDictionary<Permissions.Action, IEnumerable<string>>> GetPermissionSets();
    }
}