using System;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{
    [TestClass]
    public class LdapProviderTests
    {
        private ILdapProvider provider;

        [TestInitialize]
        public void Init()
        {
            provider = new LdapProvider(new ApplicationConfiguration())
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_an_organization_exists()
        {
            const string distinguishedName = "OU=Users,OU=Green Forest,OU=Institutions,OU=ADM,DC=example,DC=com,DC=lcl";

            Assert.IsTrue(provider.OrganizationExists(distinguishedName));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_an_organization_does_not_exists()
        {
            const string distinguishedName = "OU=Users,OU=World Of Warcraft,OU=Institutions,OU=ADM,DC=example,DC=com,DC=lcl";

            Assert.IsFalse(provider.OrganizationExists(distinguishedName));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        [ExpectedException(typeof(ArgumentException))]
        public void Empty_organization_throws()
        {
            provider.OrganizationExists(string.Empty);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_a_group_exists()
        {
            const string name = "Developer";

            Assert.IsTrue(provider.GroupExists(name));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_a_group_does_not_exists()
        {
            const string name = "Does Not Exist";

            Assert.IsFalse(provider.GroupExists(name));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory),ExpectedException(typeof(ArgumentException))]
        public void Empty_group_throws()
        {
            provider.GroupExists(string.Empty);
        }

        [TestMethod,TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_a_user_exists()
        {
            const string samAccountName = "billyp";

            Assert.IsTrue(provider.UserExists(samAccountName));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public void Can_determine_a_user_does_not_exists()
        {
            const string samAccountName = "DOESNOTEXIST";

            Assert.IsFalse(provider.UserExists(samAccountName));
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory), ExpectedException(typeof(ArgumentException))]
        public void Empty_user_throws()
        {
            provider.UserExists(string.Empty);
        }

    }
}