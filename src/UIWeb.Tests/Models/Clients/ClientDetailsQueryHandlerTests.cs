using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Clients
{
    [TestClass]
    public class ClientDetailsQueryHandlerTests
    {
        private ITenantOrganizationProvider provider;
        private ClientDetailsQueryHandler handler;
        private ISearch<Client> clients;

        [TestInitialize]
        public void Init()
        {
            clients = Substitute.For<ISearch<Client>>(); 
            provider = Substitute.For<ITenantOrganizationProvider>();
            handler = new ClientDetailsQueryHandler(clients, provider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_client_details()
        {
            var id = 42;
            var organization = new Organization { Id = id };
            var client = new Client { Id = id };
            client.AddResidency(organization);
            var query = new ClientDetailsQuery(clientId: id);

            provider.GetTenantOrganization().Returns(organization);

            var items = new[] {client}.AsAsyncQueryable();
            clients.GetById(Arg.Is(client.Id)).Returns(items);

            var result = await handler.Handle(query);

            Assert.IsInstanceOfType(result, typeof(ClientDetails));
            Assert.AreEqual(result.Client.Id, client.Id);
        }

        [TestMethod]
        public async Task Missing_client_returns_null()
        {
            var id = 42;
            var organization = new Organization { Id = id };
            var query = new ClientDetailsQuery(clientId: id);

            provider.GetTenantOrganization().Returns(organization);
            clients.GetById(Arg.Any<int>())
                .Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            var result = await handler.Handle(query);
            
            Assert.IsNull(result);
        }
    }
}
