using System.Linq;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Data
{
    [TestClass]
    public class ReceiptTypePersistenceTests
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
        public void Can_get_ReceiptTypes()
        {
            var types = context.TransactionSubTypes.OfType<ReceiptType>().ToList();

            Assert.IsFalse(types.Count == 0, "Default Receipt Types not found in the test database");
        }
    }
}