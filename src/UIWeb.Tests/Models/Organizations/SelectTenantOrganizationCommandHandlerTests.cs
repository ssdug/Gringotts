using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Organizations
{
    [TestClass]
    public class SelectTenantOrganizationCommandHandlerTests
    {
        private ITenantKeyManager tenantKeyManager;
        private ITenantUrlHelper tenantUrlHelper;
        private SelectTenantOrganizationCommandHandler handler;
        private ISearch<Organization> organizations;

        [TestInitialize]
        public void Init()
        {
            organizations = Substitute.For<ISearch<Organization>>();
            tenantKeyManager = Substitute.For<ITenantKeyManager>();
            tenantUrlHelper = Substitute.For<ITenantUrlHelper>();
            handler = new SelectTenantOrganizationCommandHandler(organizations, tenantKeyManager, tenantUrlHelper)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_select_tenant_organization()
        {
            var command = new SelectTenantOrganizationCommand("foo", "some/url");

            organizations.All()
                .Returns(new[] {new Organization {Abbreviation = "foo"}}.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            tenantKeyManager.Received()
                .SetTenantKey(command.TenantKey);

            tenantUrlHelper.Received()
                .TransformTenantUrl(command.ReturnUrl, command.TenantKey);
        }

        [TestMethod]
        public async Task selecting_invalid_tenant_organization_should_fail()
        {
            var command = new SelectTenantOrganizationCommand("bar", "some/url");

            organizations.All()
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));

            tenantKeyManager.DidNotReceive()
                .SetTenantKey(command.TenantKey);
        }
    }
}