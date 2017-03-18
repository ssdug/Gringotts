using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    [TestClass]
    public class OrganizationDetailsQueryHandlerTests
    {
        private OrganizationDetailsQueryHandler hander;
        private ISearch<Organization> organizations;

        [TestInitialize]
        public void Init()
        {
            organizations = Substitute.For<ISearch<Organization>>();
            hander = new OrganizationDetailsQueryHandler(organizations)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_organization_details()
        {
            var organizationId = 42;
            var command = new OrganizationDetailsQuery(organizationId);

            organizations.GetById(Arg.Any<int>())
                .Returns(new[] {new Organization {Id = organizationId}}.AsAsyncQueryable());

            var result = await hander.Handle(command);

            Assert.IsInstanceOfType(result, typeof(OrganizationDetails));
            Assert.AreEqual(result.Organization.Id, organizationId);
        }

        [TestMethod]
        public async Task Missing_oranizations_return_null()
        {
            var organizationId = 42;
            var command = new OrganizationDetailsQuery(organizationId);

            organizations.GetById(Arg.Any<int>())
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            var result = await hander.Handle(command);

            Assert.IsNull(result);
        }
    }
}