using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    [TestClass]
    public class PayeeEditorFormPayeeTypeValidationHandlerTests
    {
        private PayeeEditorFormPayeeTypeValidatorHandler handler;

        [TestInitialize]
        public void Init()
        {
            handler = new PayeeEditorFormPayeeTypeValidatorHandler()
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_handle_request()
        {
            var command = GetCommand(add: true);

            handler.Validators = Enumerable.Empty<Action<AddOrEditPayeeCommand>>();

            await handler.Handle(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Can_handle_invalid_request()
        {
            var command = GetCommand(add: true);

            command.ModelState.AddModelError("foo", "bar");

            handler.Validators = Enumerable.Empty<Action<AddOrEditPayeeCommand>>();

            await handler.Handle(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_SelectedTypes_fails()
        {
            var command = GetCommand(add: true);

            command.Editor.SelectedTypes = new[]
            {
                new PayeeEditorForm.PayeeType {Id = 1, Name = "foo"},
                new PayeeEditorForm.PayeeType {Id = 1, Name = "foo"},
            };

            handler.EnsurePayeeTypesAreDistinct(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Distinct_SelectedTypes_passes()
        {
            var command = GetCommand(add: true);

            command.Editor.SelectedTypes = new[]
            {
                new PayeeEditorForm.PayeeType {Id = 1, Name = "foo"},
                new PayeeEditorForm.PayeeType {Id = 2, Name = "bar"},
            };

            handler.EnsurePayeeTypesAreDistinct(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        private AddOrEditPayeeCommand GetCommand(bool add = false)
        {
            var form = GetForm(add: add);

            return new AddOrEditPayeeCommand(form, new ModelStateDictionary());
        }

        private static PayeeEditorForm GetForm(bool add = false)
        {
            return new PayeeEditorForm
            {
                PayeeId = add ? new int?() : 42,
            };
        }
    }
}
