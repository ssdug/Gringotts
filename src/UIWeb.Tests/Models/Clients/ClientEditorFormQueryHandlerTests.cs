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
    public class ClientEditorFormQueryHandlerTests
    {
        private ILookup<ClientIdentifierType> identifierTypes;
        private ClientEditorFormQueryHandler handler;
        private ITenantOrganizationProvider tenantOrganizationProvider;
        private ISearch<Client> clients;

        [TestInitialize]
        public void Init()
        {
            identifierTypes = Substitute.For<ILookup<ClientIdentifierType>>();
            clients = Substitute.For<ISearch<Client>>();
            tenantOrganizationProvider = Substitute.For<ITenantOrganizationProvider>();
            handler = new ClientEditorFormQueryHandler(clients, identifierTypes, tenantOrganizationProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_form()
        {
            var identifierTypes = new[] { new ClientIdentifierType{Id =1, Name = "foo"} };
            var query = new ClientEditorFormQuery();

            this.identifierTypes.All.Returns(identifierTypes.AsEnumerable());
            
            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ClientEditorForm));
            Assert.IsNull(result.ClientId);
        }

        [TestMethod]
        public async Task Can_get_form_for_client()
        {
            var organization = new Organization {Id = 1};
            var clientId = 42;
            var identifierTypes = new[] {new ClientIdentifierType {Id = 1, Name = "foo"}};
            var client = new Client
            {
                Id = clientId,
                Identifiers = new[]
                {
                    new ClientIdentifier {ClientIdentifierType = identifierTypes.First()}
                }
            };

            client.AddResidency(organization);

        var query = new ClientEditorFormQuery(clientId: clientId);

            tenantOrganizationProvider.GetTenantOrganization().Returns(organization);

            clients.GetById(Arg.Is(clientId)).Returns(new[] {client}.AsAsyncQueryable());

            this.identifierTypes.All.Returns(identifierTypes.AsEnumerable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ClientEditorForm));
            Assert.AreEqual(result.ClientId, client.Id);
        }

        [TestMethod]
        public async Task Can_get_form_for_missing_client()
        {
            var clientId = 42;
            var identifierTypes = new[] { new ClientIdentifierType { Id = 1, Name = "foo" } };
            
            var query = new ClientEditorFormQuery(clientId: clientId);

            clients.GetById(Arg.Is(clientId)).Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            this.identifierTypes.All.Returns(identifierTypes.AsEnumerable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ClientEditorForm));

            handler.Logger.Received().Warn(Arg.Any<string>(), clientId);
        }

        [TestMethod]
        public async Task Missing_identifiertypes_generate_warning()
        {
            var clientId = 42;
            
            var query = new ClientEditorFormQuery(clientId: clientId);

            clients.GetById(Arg.Is(clientId)).Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            identifierTypes.All.Returns(Enumerable.Empty<ClientIdentifierType>().AsEnumerable());

            await handler.Handle(query);

            handler.Logger.Received().Warn(Arg.Any<string>(), clientId);
        }
    }
}
