using System;
using System.Web;
using Autofac.Extras.NLog;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy
{
    public interface ITenantKeyManager
    {
        string GetTenantKey();
        void SetTenantKey(string value);
    }

    public class HttpCookieTenantKeyManager : ITenantKeyManager
    {
        public ILogger Logger { get; set; }

        private readonly HttpContextBase httpContext;
        private readonly bool secure;

        public HttpCookieTenantKeyManager(HttpContextBase httpContext, IApplicationConfiguration config)
        {
            this.httpContext = httpContext;
            this.secure = config.EnableSecureHttpCookies;
        }

        public string GetTenantKey()
        {
            Logger.Trace("GetTenantKey");

            var cookie = httpContext.Request.Cookies["tenant_key"];

            if (cookie == null)
            {
                Logger.Warn("tenant_key cookie was not found.");
                return null;
            }

            return cookie.Value;
        }

        public void SetTenantKey(string value)
        {
            Logger.Info("SetTenantKey::{0}", value);

            var cookie = new HttpCookie("tenant_key", value)
            {
                HttpOnly = true,
                Secure = secure,
                Expires = DateTime.UtcNow.AddYears(1)
            };

            httpContext.Response.Cookies.Add(cookie);
        }
    }
}