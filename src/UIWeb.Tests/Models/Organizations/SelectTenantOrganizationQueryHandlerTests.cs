using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    [TestClass]
    public class SelectTenantOrganizationQueryHandlerTests
    {
        private ITenantOrganizationProvider tentantOrganizationProvider;
        private SelectTenantOrganizationQueryHandler handler;

        [TestInitialize]
        public void Init()
        {
            tentantOrganizationProvider = Substitute.For<ITenantOrganizationProvider>();
            handler = new SelectTenantOrganizationQueryHandler(tentantOrganizationProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_tenant_list()
        {
            var returnUrl = "someurl";
            var tenantOrganizations = Enumerable.Empty<Organization>();
            var query = new SelectTenantOrganizationQuery(returnUrl);

            tentantOrganizationProvider.GetAllTenantOrganizationsForUser()
                .Returns(tenantOrganizations);

            var result = await handler.Handle(query);

            Assert.AreEqual(result.ReturnUrl, returnUrl);
            Assert.AreEqual(result.TenantOrganizations, tenantOrganizations);
        }
    }
}