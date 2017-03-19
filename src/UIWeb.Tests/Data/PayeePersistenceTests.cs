using System.Linq;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Data
{
    [TestClass]
    public class PayeePersistenceTests
    {
        private ApplicationDbContext context;
        private Organization organization;

        [TestInitialize]
        public void Init()
        {
            context = TestDbContextFactory.Build();
            organization = context.Organizations.First();
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.Delete();
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Payee()
        {
            var payee = new Payee
            {
                Id = -1,
                Name = "Heirloom Vendor",
                AddressLine1 = "123 Street",
                City = "Stormwind",
                State = "WA",
                PostalCode = "11111",
                Phone = "360-555-1212",
                UpdatedBy = "Foo",
                CreatedBy = "Foo",
                Organization = organization
            };

            context.Payees.Add(payee);
            context.SaveChanges();

            Assert.AreNotEqual(-1, payee.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Payee_with_type()
        {
            var type = context.PayeeTypes.First();

            var payee = new Payee
            {
                Id = -1,
                Name = "Heirloom Vendor",
                AddressLine1 = "123 Street",
                City = "Stormwind",
                State = "WA",
                PostalCode = "11111",
                Phone = "360-555-1212",
                UpdatedBy = "Foo",
                CreatedBy = "Foo",
                Types = new[] { type },
                Organization = organization
            };

            context.Payees.Add(payee);
            context.SaveChanges();

            Assert.AreNotEqual(-1, payee.Id);
        }
    }
}