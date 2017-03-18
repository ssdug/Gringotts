using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    [TestClass]
    public class ClientEditorFormIdentifierValidationHandlerTests
    {
        private ApplicationDbContext context;
        private ClientEditorFormIdentifierValidatorHandler handler;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
            handler = new ClientEditorFormIdentifierValidatorHandler(context)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_handle_request()
        {
            var command = GetCommand(add: true);

            handler.Validators = Enumerable.Empty<Action<AddOrEditClientCommand>>();

            await handler.Handle(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Can_handle_invalid_request()
        {
            var command = GetCommand(add: true);

            command.ModelState.AddModelError("foo", "bar");

            handler.Validators = Enumerable.Empty<Action<AddOrEditClientCommand>>();

            await handler.Handle(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_IdentifierTypes_fails()
        {
            var command = GetCommand(add: true);

            command.Editor.Identifiers = new[]
            {
                new ClientEditorForm.Identifier {TypeId = 1, Value = "foo"},
                new ClientEditorForm.Identifier {TypeId = 1, Value = "bar"}
            };

            handler.EnsureIdentifierTypesAreDistinct(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Distinct_IdentifierTypes_passes()
        {
            var command = GetCommand(add: true);

            command.Editor.Identifiers = new[]
            {
                new ClientEditorForm.Identifier {TypeId = 1, Value = "foo"},
                new ClientEditorForm.Identifier {TypeId = 2, Value = "bar"}
            };

            handler.EnsureIdentifierTypesAreDistinct(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Invalid_SSN_fails()
        {
            var identifierType = new ClientEditorForm.IdentifierType {Id = 1, Name = "SSN"};
            var command = GetCommand(add: true);

            command.Editor.IdentifierTypes = new[] {identifierType};
            command.Editor.Identifiers = new[]
            {
                new ClientEditorForm.Identifier {TypeId = 1, Value = "foo"}
            };

            handler.EnsureSSNIdentifiersAreValid(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Valid_SSN_passes()
        {
            var identifierType = new ClientEditorForm.IdentifierType { Id = 1, Name = "SSN" };
            var command = GetCommand(add: true);

            command.Editor.IdentifierTypes = new[] { identifierType };
            command.Editor.Identifiers = new[]
            {
                new ClientEditorForm.Identifier {TypeId = 1, Value = "000-00-0000"}
            };

            handler.EnsureSSNIdentifiersAreValid(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_Identifiers_fails()
        {
            var identifier = new ClientEditorForm.Identifier {TypeId = 1, Value = "foo"};
            var clientIdentifer = new ClientIdentifier
            {
                Client = new Client{Id = 24 },
                ClientIdentifierType = new ClientIdentifierType {Id = 1},
                Value = "foo"
            };
            var command = GetCommand(add: true);

            command.Editor.ClientId = 42;
            command.Editor.Identifiers = new[] {identifier};

            context.ClientIdentifiers = Substitute.For<IDbSet<ClientIdentifier>, IDbAsyncEnumerable<ClientIdentifier>>()
                .Initialize(new[] { clientIdentifer }.AsQueryable());

            handler.EnsureIdentifiersAreNotInUse(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Unique_Identifiers_passes()
        {
            var identifier = new ClientEditorForm.Identifier { TypeId = 1, Value = "foo" };
            var clientIdentifer = new ClientIdentifier
            {
                Client = new Client { Id = 24 },
                ClientIdentifierType = new ClientIdentifierType { Id = 1 },
                Value = "bar"
            };
            var command = GetCommand(add: true);

            command.Editor.ClientId = 42;
            command.Editor.Identifiers = new[] { identifier };

            context.ClientIdentifiers = Substitute.For<IDbSet<ClientIdentifier>, IDbAsyncEnumerable<ClientIdentifier>>()
                .Initialize(new[] { clientIdentifer }.AsQueryable());

            handler.EnsureIdentifiersAreNotInUse(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_Identifiers_for_same_client_passes()
        {
            var identifier = new ClientEditorForm.Identifier { TypeId = 1, Value = "foo" };
            var clientIdentifer = new ClientIdentifier
            {
                Client = new Client { Id = 42 },
                ClientIdentifierType = new ClientIdentifierType { Id = 1 },
                Value = "foo"
            };
            var command = GetCommand(add: true);

            command.Editor.ClientId = 42;
            command.Editor.Identifiers = new[] { identifier };

            context.ClientIdentifiers = Substitute.For<IDbSet<ClientIdentifier>, IDbAsyncEnumerable<ClientIdentifier>>()
                .Initialize(new[] { clientIdentifer }.AsQueryable());

            handler.EnsureIdentifiersAreNotInUse(command);

            Assert.IsTrue(command.ModelState.IsValid);
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