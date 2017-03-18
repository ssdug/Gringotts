using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Autofac.Extras.NLog;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Security
{
    public interface IPrincipalProvider
    {
        IPrincipal GetCurrent();
    }

    public class HttpContextPrincipalProvider : IPrincipalProvider
    {
        public ILogger Logger { get; set; }

        private readonly IApplicationConfiguration configuration;
        private readonly HttpContextBase context;

        public HttpContextPrincipalProvider(IApplicationConfiguration configuration, HttpContextBase context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public IPrincipal GetCurrent()
        {
            Logger.Trace("GetCurrent");

            if (!configuration.OverrideGroups.Any()) 
                return context.User;

            Logger.Warn("override groups in use: {0}", configuration.OverrideGroups.JoinWithComma());

            return new OverrideClaimsPrincipal((ClaimsPrincipal)context.User, configuration.OverrideGroups)
            {
                Logger = Logger
            };
        }

        private class OverrideClaimsPrincipal : IPrincipal
        {
            public ILogger Logger { get; set; }

            private readonly ClaimsPrincipal principal;
            private readonly IEnumerable<string> additionalRoles;

            public OverrideClaimsPrincipal(ClaimsPrincipal principal, IEnumerable<string> additionalRoles)
            {
                this.principal = principal;
                this.additionalRoles = additionalRoles;
            }

            public bool IsInRole(string role)
            {
                var result = additionalRoles.Any(r => r.Equals(role, StringComparison.InvariantCultureIgnoreCase))
                    || principal.IsInRole(role);

                Logger.Info("OverrideClaimsPrincipal::IsInRole::{0}::{1}", role, result);

                return result;
            }

            public IIdentity Identity
            {
                get { return principal.Identity; }
            }
        }
    }
}
