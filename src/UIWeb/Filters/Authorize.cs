using System;
using System.Linq;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;

namespace Wiz.Gringotts.UIWeb.Filters
{


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class Authorize : AuthorizeAttribute
    {
        public Permissions Permissions { get; set; }
        public IPrincipalProvider PrincipalProvider { get; set; }

        public ILogger Logger { get; set; }

        private readonly Type resourceType;
        private readonly Permissions.Action[] actions;

        public Authorize(Type resourceType, params Permissions.Action[] actions)
        {
            this.resourceType = resourceType;
            this.actions = actions;
        }

        public override void OnAuthorization(AuthorizationContext context)
        {
            Logger.Trace("OnAuthorization");

            if (Permissions == null)
            {
                throw new InvalidOperationException("No permission sets found");
            }

            if (PrincipalProvider == null)
            {
                throw new InvalidOperationException("No principal provider found");
            }

            var user = PrincipalProvider.GetCurrent();
            var authorized = actions.Any(action => Permissions.CanPerform(user, resourceType, action));

            if (authorized) 
                return;

            context.Result = new HttpForbiddenResult();
            
            Logger.Warn("unauthorized access detected by {0}", 
                user == null ? "Anonymous" : user.Identity.Name);
        }
    }
}