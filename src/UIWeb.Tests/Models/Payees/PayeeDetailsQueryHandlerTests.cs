using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    [TestClass]
    public class PayeeDetailsQueryHandlerTests
    {
        private ITenantOrganizationProvider tenantOrganizationProvider;
        private PayeeDetailsQueryHandler handler;
        private ISearch<Client> clients;
        private ISearch<Payee> payees;

        [TestInitialize]
        public void Init()
        {
            tenantOrganizationProvider = Substitute.For<ITenantOrganizationProvider>();
            payees = Substitute.For<ISearch<Payee>>();
            clients = Substitute.For<ISearch<Client>>();
            handler = new PayeeDetailsQueryHandler(payees, clients, tenantOrganizationProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_payee_details()
        {
            var organizationId = 24;
            var organization = new Organization {Id = organizationId};

            tenantOrganizationProvider.GetTenantOrganization().Returns(organization);

            var payeeId = 42;
            var payee = new Payee {Id = payeeId, Organization = organization, OrganizationId = organizationId};
            var query = new PayeeDetailsQuery(payeeId: payeeId);

            payees.GetById(Arg.Is(payeeId))
                .Returns(new[] {payee}.AsAsyncQueryable());

            clients.All()
                .Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsInstanceOfType(result, typeof(PayeeDetails));
            Assert.AreEqual(result.Payee.Id, payee.Id);
        }

        [TestMethod]
        public async Task Missing_payee_returns_null()
        {
            var payeeId = 42;
            var query = new PayeeDetailsQuery(payeeId: payeeId);

            payees.GetById(Arg.Is(payeeId))
                .Returns(Enumerable.Empty<Payee>().AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsNull(result);
        }
    }
}