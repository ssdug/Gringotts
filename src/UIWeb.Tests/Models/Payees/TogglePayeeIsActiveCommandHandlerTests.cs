using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    [TestClass]
    public class TogglePayeeIsActiveCommandHandlerTests
    {
        private ApplicationDbContext context;
        private TogglePayeeIsActiveCommandHandler handler;
        private ISearch<Payee> payees;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
            payees = Substitute.For<ISearch<Payee>>();
            handler = new TogglePayeeIsActiveCommandHandler(payees, context)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_toggle_payee_active()
        {
            var payeeId = 42;
            var payee = new Payee {Id = payeeId, IsActive = true};
            var command = new TogglePayeeIsActiveCommand(payeeId: payeeId);

            payees.GetById(Arg.Is(payeeId))
                .Returns(new[] {payee}.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsFalse(payee.IsActive);
        }

        [TestMethod]
        public async Task Can_toggle_payee_inactive()
        {
            var payeeId = 42;
            var payee = new Payee { Id = payeeId, IsActive = false };
            var command = new TogglePayeeIsActiveCommand(payeeId: payeeId);

            payees.GetById(Arg.Is(payeeId))
                .Returns(new[] { payee }.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsTrue(payee.IsActive);
        }

        [TestMethod]
        public async Task Toggle_missing_payee_fails()
        {
            var payeeId = 42;
            var command = new TogglePayeeIsActiveCommand(payeeId: payeeId);

            payees.GetById(Arg.Is(payeeId))
                .Returns(Enumerable.Empty<Payee>().AsAsyncQueryable());
            
            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));
        }
    }
}