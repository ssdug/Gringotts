using System.Configuration;
using System.Linq;
using Wiz.Gringotts.UIWeb.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Infrastructure
{
    [TestClass]
    public class ApplicationConfigurationTests
    {
        private IApplicationConfiguration config;

        [TestInitialize]
        public void Init()
        {
            config = new ApplicationConfiguration();   
        }

        [TestMethod]
        public void Can_get_connection_string()
        {
            Assert.AreEqual(config.SqlConnectionString, ConfigurationManager.ConnectionStrings["DefaultConnection"]);
        }

        [TestMethod]
        public void Can_get_cache_expiration()
        {
            Assert.AreEqual(config.CacheExpirationTimeInMinutes, int.Parse(ConfigurationManager.AppSettings["app:cache_expiration_time_in_minutes"]));
        }

        [TestMethod]
        public void Can_get_ldap_path()
        {
            Assert.AreEqual(config.LdapPath, ConfigurationManager.AppSettings["app:ldap_path"]);
        }


        [TestMethod]
        public void Can_get_glipmse_enabled()
        {
            Assert.AreEqual(config.EnableGlimpse, bool.Parse(ConfigurationManager.AppSettings["app:enable_glimpse"]));
        }

        [TestMethod]
        public void Can_get_nlog_enabled()
        {
            Assert.AreEqual(config.EnableNLog, bool.Parse(ConfigurationManager.AppSettings["app:enable_nlog"]));
        }

        [TestMethod]
        public void Can_get_secure_http_cookie_enabled()
        {
            Assert.AreEqual(config.EnableSecureHttpCookies, bool.Parse(ConfigurationManager.AppSettings["app:enable_secure_http_cookies"]));
        }

        [TestMethod]
        public void Can_get_override_groups()
        {
            var groups = config.OverrideGroups;

            Assert.IsNotNull(groups);
            Assert.IsTrue(groups.Any());
        }

    }
}
