using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    [TestClass]
    public class ClientEditorFormPayeesValidationHandlerTests
    {
        private ApplicationDbContext context;
        private ClientEditorFormPayeesValidatorHandler handler;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
            handler = new ClientEditorFormPayeesValidatorHandler(context)
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
        public void Duplicate_Attorneys_fails()
        {
            var command = GetCommand(add: true);

            command.Editor.Attorneys = new[]
            {
                new ClientEditorForm.Payee { Id = 1, Name = "foo"},
                new ClientEditorForm.Payee { Id = 1, Name = "foo"}
            };

            handler.EnsureAttorneysAreUnique(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Distinct_Attorneys_passes()
        {
            var command = GetCommand(add: true);

            command.Editor.Attorneys = new[]
            {
                new ClientEditorForm.Payee { Id = 1, Name = "foo"},
                new ClientEditorForm.Payee { Id = 2, Name = "bar"}
            };

            handler.EnsureAttorneysAreUnique(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_Guardians_fails()
        {
            var command = GetCommand(add: true);

            command.Editor.Guardians = new[]
            {
                new ClientEditorForm.Payee { Id = 1, Name = "foo"},
                new ClientEditorForm.Payee { Id = 1, Name = "foo"}
            };

            handler.EnsureGuardiansAreUnique(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Distinct_Guardians_passes()
        {
            var command = GetCommand(add: true);

            command.Editor.Guardians = new[]
            {
                new ClientEditorForm.Payee { Id = 1, Name = "foo"},
                new ClientEditorForm.Payee { Id = 2, Name = "bar"}
            };

            handler.EnsureGuardiansAreUnique(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Payee_as_both_Attorney_and_Guardian_passes()
        {
            var payee = new ClientEditorForm.Payee {Id = 1, Name = "foo"};
            var command = GetCommand(add: true);

            command.Editor.Guardians = new[] { payee };
            command.Editor.Attorneys = new[] { payee };

            handler.Handle(command);

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