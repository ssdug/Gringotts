using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NExtensions;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    [TestClass]
    public class ToggleClientIsActiveCommandHandlerTests
    {
        private ApplicationDbContext context;
        private ToggleClientIsActiveCommandHandler handler;
        private ITenantOrganizationProvider tenantOrganizationProvider;
        private Organization organization;
        private ISearch<Client> clients;

        [TestInitialize]
        public void Init()
        {
            organization = new Organization {Id = 42};
            tenantOrganizationProvider = Substitute.For<ITenantOrganizationProvider>();
            tenantOrganizationProvider.GetTenantOrganization().Returns(organization);
            context = Substitute.For<ApplicationDbContext>();
            clients = Substitute.For<ISearch<Client>>();

            handler = new ToggleClientIsActiveCommandHandler(clients, context, tenantOrganizationProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_toggle_client_eabled()
        {
            var clientId = 42;
            var client = new Client { Id = clientId };
            client.AddResidency(organization);
            var command = new ToggleClientIsActiveCommand(clientId: clientId);

            clients.GetById(Arg.Is(client.Id)).Returns(new[] { client }.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsFalse(client.Residencies.First().IsActive);
        }

        [TestMethod]
        public async Task Can_toggle_client_disabled()
        {
            var clientId = 42;
            var client = new Client { Id = clientId };
            client.AddResidency(organization);
            client.Residencies.ForEach(r => r.IsActive = false);
            var command = new ToggleClientIsActiveCommand(clientId: clientId);

            clients.GetById(Arg.Is(client.Id)).Returns(new[] { client }.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsTrue(client.Residencies.First().IsActive);
        }

        [TestMethod]
        public async Task Toggle_missing_client_fails()
        {
            var clientId = 42;
            var command = new ToggleClientIsActiveCommand(clientId: clientId);

            clients.GetById(Arg.Is(clientId)).Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));

        }
    }
}
