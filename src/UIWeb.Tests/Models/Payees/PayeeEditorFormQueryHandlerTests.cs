using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Payees
{
    [TestClass]
    public class PayeeEditorFormQueryHandlerTests
    {
        private ILookup<PayeeType> payeeTypes;
        private PayeeEditorFormQueryHandler handler;
        private ISearch<Payee> payees;

        [TestInitialize]
        public void Init()
        {
            payeeTypes = Substitute.For<ILookup<PayeeType>>();
            payees = Substitute.For<ISearch<Payee>>();
            handler = new PayeeEditorFormQueryHandler(payees, payeeTypes)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_get_form()
        {
            var query = new PayeeEditorFormQuery();

            payeeTypes.All.Returns(Enumerable.Empty<PayeeType>().AsEnumerable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PayeeEditorForm));
            Assert.IsNull(result.PayeeId);
        }

        [TestMethod]
        public async Task Can_get_form_with_available_payee_types()
        {
            var payeeType = new PayeeType {Id = 1, Name = "Foo"};
            var query = new PayeeEditorFormQuery();

            payeeTypes.All.Returns(new [] { payeeType }.AsEnumerable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PayeeEditorForm));
            Assert.IsTrue(result.AvailableTypes.Any(a => a.Id == payeeType.Id));
            Assert.IsNull(result.PayeeId);
        }

        [TestMethod]
        public async Task Can_get_form_for_payee()
        {
            var payeeId = 42;
            var payee = new Payee
            {
                Id = payeeId
            };
            var payeeType = new PayeeType { Id = 1, Name = "Foo" };
            var query = new PayeeEditorFormQuery(payeeId: payeeId);

            payeeTypes.All.Returns(new[] { payeeType }.AsEnumerable());

            payees.GetById(Arg.Is(payeeId))
                .Returns(new[] {payee}.AsAsyncQueryable());
           
            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PayeeEditorForm));
            Assert.IsTrue(result.AvailableTypes.Any(a => a.Id == payeeType.Id));
            Assert.AreEqual(result.PayeeId, payee.Id);
        }

        [TestMethod]
        public async Task Payee_not_found()
        {
            var payeeId = 42;
            var payeeType = new PayeeType { Id = 1, Name = "Foo" };
            var query = new PayeeEditorFormQuery(payeeId: payeeId);

            payeeTypes.All.Returns(new[] { payeeType }.AsEnumerable());

            payees.GetById(Arg.Is(payeeId))
                .Returns(Enumerable.Empty<Payee>().AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PayeeEditorForm));
            Assert.IsTrue(result.AvailableTypes.Any(a => a.Id == payeeType.Id));

            handler.Logger.Received().Warn(Arg.Any<string>(), payeeId);
        }
    }
}