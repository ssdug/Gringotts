using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Clients
{
    [TestClass]
    public class AddOrEditClientCommandHandlerTests
    {
        private ApplicationDbContext context;
        private AddOrEditClientCommandHandler handler;
        private ITenantOrganizationProvider tentantOrganizationProvider;
        private Organization organization;

        [TestInitialize]
        public void Init()
        {
            organization = new Organization {Id = 42};
            tentantOrganizationProvider = Substitute.For<ITenantOrganizationProvider>();
            tentantOrganizationProvider.GetTenantOrganization().Returns(organization);
            context = Substitute.For<ApplicationDbContext>();
            handler = new AddOrEditClientCommandHandler(context, tentantOrganizationProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_add_client()
        {
            var identifierTypes = new[] { new ClientIdentifierType{Id =1, Name = "foo"} };
            var command = GetCommand(add: true);

            context.ClientIdentifierTypes = Substitute.For<IDbSet<ClientIdentifierType>, IDbAsyncEnumerable<ClientIdentifierType>>()
                .Initialize(identifierTypes.AsQueryable());

            context.Payees = Substitute.For<IDbSet<Payee>, IDbAsyncEnumerable<Payee>>()
                .Initialize(Enumerable.Empty<Payee>().AsQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            context.Clients.Received()
                .Add(Arg.Any<Client>());

            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Can_edit_client()
        {
            var organization = new Organization { Id = 1 };
            var residency = new Residency { Id = 2, Organization = organization, OrganizationId = organization.Id};
            var identifierTypes = new[] { new ClientIdentifierType { Id = 1, Name = "foo" } };
            var client = new Client {Id = 42, Residencies = new[] {residency}, Identifiers = Enumerable.Empty<ClientIdentifier>().ToList()};
            var command = GetCommand(add: false);

            tentantOrganizationProvider.GetTenantOrganization().Returns(organization);

            context.Clients = Substitute.For<IDbSet<Client>, IDbAsyncEnumerable<Client>>()
                .Initialize(new []{client}.AsQueryable());

            context.ClientIdentifierTypes = Substitute.For<IDbSet<ClientIdentifierType>, IDbAsyncEnumerable<ClientIdentifierType>>()
                .Initialize(identifierTypes.AsQueryable());

            context.Payees = Substitute.For<IDbSet<Payee>, IDbAsyncEnumerable<Payee>>()
                .Initialize(Enumerable.Empty<Payee>().AsQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));


            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Invalid_model_state_fails()
        {
            var command = GetCommand(add: true);

            command.ModelState.AddModelError("","error");

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));

        }

        private AddOrEditClientCommand GetCommand(bool add = false)
        {
            var form = GetForm(add: add);

            return new AddOrEditClientCommand(form, new ModelStateDictionary());
        }

        private static ClientEditorForm GetForm(bool add = false)
        {
            return new ClientEditorForm
            {
                ClientId = add ? new int?() : 42,
            };
        }
    }
}