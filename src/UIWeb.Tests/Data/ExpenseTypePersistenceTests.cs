using System.Linq;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Data
{
    [TestClass]
    public class ExpenseTypePersistenceTests
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
        public void Can_get_ExpenseTypes()
        {
            var types = context.TransactionSubTypes.OfType<ExpenseType>().ToList();

            Assert.IsFalse(types.Count == 0, "Default Expense Types not found in the test database");
        }
    }
}