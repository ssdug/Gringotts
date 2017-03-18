using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Wiz.Gringotts.UIWeb.Infrastructure
{
    public interface IApplicationConfiguration
    {
        ConnectionStringSettings SqlConnectionString { get; }
        string LdapPath { get; }
        bool EnableGlimpse { get; }
        bool EnableNLog { get; }
        bool EnableSecureHttpCookies { get; }
        IEnumerable<string> OverrideGroups { get; }
        int CacheExpirationTimeInMinutes { get; }
    }

    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public ConnectionStringSettings SqlConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"]; }
        }

        public int CacheExpirationTimeInMinutes { get { return int.Parse(ConfigurationManager.AppSettings["app:cache_expiration_time_in_minutes"]); } }

        public string LdapPath { get { return (ConfigurationManager.AppSettings["app:ldap_path"] ?? string.Empty); } }
        public bool EnableGlimpse { get { return bool.Parse(ConfigurationManager.AppSettings["app:enable_glimpse"]); } }
        public bool EnableNLog { get { return bool.Parse(ConfigurationManager.AppSettings["app:enable_nlog"]); } }
        public bool EnableSecureHttpCookies { get { return bool.Parse(ConfigurationManager.AppSettings["app:enable_secure_http_cookies"]); } }
        public IEnumerable<string> OverrideGroups { get { return (ConfigurationManager.AppSettings["override:groups"] ?? string.Empty).Split(';').Where(x => x.Length > 0); } }
    }
}
