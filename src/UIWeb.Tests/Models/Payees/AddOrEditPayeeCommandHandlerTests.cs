using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Payees
{
    [TestClass]
    public class AddOrEditPayeeCommandHandlerTests
    {
        private ApplicationDbContext context;
        private AddOrEditPayeeCommandHandler handler;
        private ISearch<Payee> payees;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
            payees = Substitute.For<ISearch<Payee>>();
            handler = new AddOrEditPayeeCommandHandler(payees, context)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_add_payee()
        {
            var command = GetCommand(add: true);

            context.PayeeTypes = Substitute.For<IDbSet<PayeeType>, IDbAsyncEnumerable<PayeeType>>()
                .Initialize(Enumerable.Empty<PayeeType>().AsQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            context.Payees.Received()
                .Add(Arg.Any<Payee>());

            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Can_edit_payee()
        {
            var payee = new Payee {Id = 42};
            var command = GetCommand(add: false);
            payees.GetById(Arg.Any<int>())
                .Returns(new[] {payee}.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Invalid_model_state_fails()
        {
            var command = GetCommand(add: true);

            command.ModelState.AddModelError("", "error");

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));

        }

        public AddOrEditPayeeCommand GetCommand(bool add = false)
        {
            var form = GetForm(add: add);

            return new AddOrEditPayeeCommand(form, new ModelStateDictionary());
        }

        public PayeeEditorForm GetForm(bool add = false)
        {
            return new PayeeEditorForm
            {
                PayeeId = add ? new int?() : 42 
            };
        }
    }
}