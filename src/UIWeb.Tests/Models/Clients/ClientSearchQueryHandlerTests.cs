using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Clients
{
    [TestClass]
    public class ClientSearchQueryHandlerTests
    {

        private ClientSearchQueryHandler handlerHandler;
        private ISearch<Client> clients;

        [TestInitialize]
        public void Init()
        {
            clients = Substitute.For<ISearch<Client>>();
            handlerHandler = new ClientSearchQueryHandler(clients)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_search_clients_with_results()
        {
            var id = 42;
            var client = new Client {Id = id, LastName = "foo", FirstName = "bar", 
                Identifiers = Enumerable.Empty<ClientIdentifier>().ToList()};

            var pager = new SearchPager {Search = "s"};
            var query = new ClientSearchQuery(pager);
   
            clients.GetBySearch(Arg.Is(pager)).Returns(new[] { client }.AsAsyncQueryable());

            var result = await handlerHandler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ClientSearchResult));
            Assert.IsTrue(result.Items.Contains(client));
        }

        [TestMethod]
        public async Task Can_search_clients_with_no_results()
        {
            var pager = new SearchPager { Search = "s" };
            var query = new ClientSearchQuery(pager);

            clients.GetBySearch(Arg.Is(pager)).Returns(Enumerable.Empty<Client>().AsAsyncQueryable());

            var result = await handlerHandler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ClientSearchResult));
            Assert.IsFalse(result.Items.Any());
        }

        //TODO: Migrate to ISearch<T> tests
        //[TestMethod]
        //public async Task Can_search_disabled_clients()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var client = new Client
        //    {
        //        Id = id,
        //        LastName = "foo",
        //        FirstName = "bar",
        //        Identifiers = Enumerable.Empty<ClientIdentifier>().ToList()
        //    };
        //    client.AddResidency(organization);
        //    client.Residencies.ForEach(r => r.IsActive = false);
        //    var pager = new SearchPager { Search = "f", IsActive = false };
        //    var query = new ClientSearchQuery(pager);

        //    provider.GetTenantOrganization().Returns(organization);

        //    clients.GetById(Arg.Is(id)).Returns(new[] { client }.AsAsyncQueryable());

        //    var result = await handlerHandler.Handle(query);

        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(ClientSearchResult));
        //    Assert.IsTrue(result.Items.Contains(client));
        //}

        //[TestMethod]
        //public async Task Can_search_clients_by_partial_last_name()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var client = new Client
        //    {
        //        Id = id,
        //        LastName = "foo",
        //        FirstName = "bar",
        //        Identifiers = Enumerable.Empty<ClientIdentifier>().ToList()
        //    };
        //    client.AddResidency(organization);
        //    var pager = new SearchPager { Search = "f" };
        //    var query = new ClientSearchQuery(pager);

        //    provider.GetTenantOrganization().Returns(organization);
        //    context.Clients = Substitute.For<IDbSet<Client>, IDbAsyncEnumerable<Client>>()
        //        .Initialize(new[] { client }.AsQueryable());

        //    var result = await handlerHandler.Handle(query);

        //    Assert.IsTrue(result.Items.Contains(client));
        //}

        //[TestMethod]
        //public async Task Can_search_clients_by_partial_first_name()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var client = new Client
        //    {
        //        Id = id,
        //        LastName = "foo",
        //        FirstName = "bar",
        //        Identifiers = Enumerable.Empty<ClientIdentifier>().ToList()
        //    };
        //    client.AddResidency(organization);
        //    var pager = new SearchPager { Search = "b" };
        //    var query = new ClientSearchQuery(pager);

        //    provider.GetTenantOrganization().Returns(organization);
        //    context.Clients = Substitute.For<IDbSet<Client>, IDbAsyncEnumerable<Client>>()
        //        .Initialize(new[] { client }.AsQueryable());

        //    var result = await handlerHandler.Handle(query);

        //    Assert.IsTrue(result.Items.Contains(client));
        //}

        //[TestMethod]
        //public async Task Can_search_clients_by_complete_identifier_value()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var client = new Client
        //    {
        //        Id = id,
        //        LastName = "foo",
        //        FirstName = "bar",
        //        Identifiers = new List<ClientIdentifier>
        //        {
        //            new ClientIdentifier{ Value = "123" }
        //        }
        //    };
        //    client.AddResidency(organization);
        //    var pager = new SearchPager { Search = "123" };
        //    var query = new ClientSearchQuery(pager);

        //    provider.GetTenantOrganization().Returns(organization);
        //    context.Clients = Substitute.For<IDbSet<Client>, IDbAsyncEnumerable<Client>>()
        //        .Initialize(new[] { client }.AsQueryable());

        //    var result = await handlerHandler.Handle(query);

        //    Assert.IsTrue(result.Items.Contains(client));
        //}

        //[TestMethod]
        //public async Task Cannot_search_clients_by_partial_identifier_value()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var client = new Client
        //    {
        //        Id = id,
        //        LastName = "foo",
        //        FirstName = "bar",
        //        Identifiers = new List<ClientIdentifier>
        //        {
        //            new ClientIdentifier{ Value = "123" }
        //        }
        //    };
        //    client.AddResidency(organization);
        //    var pager = new SearchPager { Search = "1" };
        //    var query = new ClientSearchQuery(pager);

        //    provider.GetTenantOrganization().Returns(organization);
        //    context.Clients = Substitute.For<IDbSet<Client>, IDbAsyncEnumerable<Client>>()
        //        .Initialize(new[] { client }.AsQueryable());

        //    var result = await handlerHandler.Handle(query);

        //    Assert.IsFalse(result.Items.Contains(client));
        //}
    }
}
