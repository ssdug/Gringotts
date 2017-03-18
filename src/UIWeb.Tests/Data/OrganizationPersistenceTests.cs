using System.Linq;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Data
{
    [TestClass]
    public class OrganizationPersistenceTests
    {
        private ApplicationDbContext context;

        [TestInitialize]
        public void Init()
        {
            context = TestDbContextFactory.Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.Delete();
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Organization()
        {
            var organization = new Organization
            {
                Id = -1,
                GroupName = "foo1",
                Name = "foo1",
                Abbreviation = "foo1",
                AddressLine1 = "foo1",
                City = "foo1",
                State = "foo1",
                PostalCode = "foo1",
                Phone = "260-555-1212",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "johnsb",
                CreatedBy = "foo",
                UpdatedBy = "foo"
            };

            context.Organizations.Add(organization);
            context.SaveChanges();

            Assert.AreNotEqual(-1, organization.Id);

            var childorg = new Organization
            {
                Id = -1,
                GroupName = "foo2",
                Name = "foo2",
                Abbreviation = "foo2",
                AddressLine1 = "foo2",
                City = "foo2",
                State = "foo2",
                PostalCode = "foo2",
                Phone = "260-555-1212",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "johnsb",
                CreatedBy = "foo",
                UpdatedBy = "foo",
                Parent = organization
            };

            context.Organizations.Add(childorg);
            context.SaveChanges();

            Assert.AreNotEqual(-1, childorg.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Organization_with_Features()
        {
            var features = context.Features.ToList();

            var organization = new Organization
            {
                Id = -1,
                GroupName = "foo1",
                Name = "foo1",
                Abbreviation = "foo1",
                AddressLine1 = "foo1",
                City = "foo1",
                State = "foo1",
                PostalCode = "foo1",
                Phone = "260-555-1212",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "johnsb",
                CreatedBy = "foo",
                UpdatedBy = "foo",
                Features = features
            };

            context.Organizations.Add(organization);
            context.SaveChanges();

            var org = context.Organizations
                .FirstOrDefault(o => o.Features.Any());
            
            Assert.IsTrue(org.Features.Count != 0);
            Assert.AreEqual(org.Features.Count, features.Count);
        }
    }
}