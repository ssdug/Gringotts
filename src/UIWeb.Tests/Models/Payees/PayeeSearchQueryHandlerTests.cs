using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Payees
{
    [TestClass]
    public class PayeeSearchQueryHandlerTests
    {
        private PayeeSearchQueryHandler handler;
        private ISearch<Payee> payees;

        [TestInitialize]
        public void Init()
        {
            payees = Substitute.For<ISearch<Payee>>();
            handler = new PayeeSearchQueryHandler(payees)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_search_payees()
        {
            var id = 42;
            var organization = new Organization { Id = id };
            var payee = new Payee { Id = id, Name = "foo", Organization = organization };
            var pager = new PayeesSearchPager {Search = "s"};
            var query = new PayeeSearchQuery(pager);

            payees.GetBySearch(pager)
                .Returns(new[] {payee}.AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PayeeSearchResult));
            Assert.IsTrue(result.Items.Contains(payee));
        }

        //TODO: migrate these tests to an ISearch<Payee> tests
        //[TestMethod]
        //public async Task Can_search_disabled_payees()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var payee = new Payee { Id = id, Name = "foo", IsActive = false, Organization = organization, Types = new PayeeType[] {}};
        //    var pager = new PayeesSearchPager { Search = "s", IsActive = false };
        //    var query = new PayeeSearchQuery(pager);

        //    payees.GetBySearch(pager)
        //        .Returns(new[] { payee }.AsAsyncQueryable());

        //    var result = await handler.Handle(query);

        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(PayeeSearchResult));
        //    Assert.IsFalse(result.Items.Any());
        //}

        //[TestMethod]
        //public async Task Can_search_payees_by_partial_name()
        //{
        //    var id = 42;
        //    var organization = new Organization { Id = id };
        //    var payee = new Payee { Id = id, Name = "foo", Organization = organization };
        //    var pager = new PayeesSearchPager { Search = "f" };
        //    var query = new PayeeSearchQuery(pager);

        //    payees.GetBySearch(pager)
        //        .Returns(new[] { payee }.AsAsyncQueryable());

        //    var result = await handler.Handle(query);

        //    Assert.IsTrue(result.Items.Any());
        //}
    }
}