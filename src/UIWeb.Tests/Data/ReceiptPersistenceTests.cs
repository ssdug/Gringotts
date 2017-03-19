using System.Linq;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Data
{
    [TestClass]
    public class ReceiptPersistenceTests
    {
        private ApplicationDbContext context;
        private Organization organization;
        private SubsidiaryFund fund;
        private SubsidiaryAccount account;
        private ReceiptType receiptType;

        [TestInitialize]
        public void Init()
        {
            context = TestDbContextFactory.Build();
            organization = context.Organizations.First();
            receiptType = context.TransactionSubTypes.OfType<ReceiptType>().First();

            var accountType = context.AccountTypes.First();
            var fundType = context.FundTypes.First();

            account = new SubsidiaryAccount
            {
                Id = -1,
                Name = "Test Account",
                UpdatedBy = "Foo",
                CreatedBy = "Foo",
                AccountType = accountType,
                Organization = organization
            };

            fund = new SubsidiaryFund
            {
                Id = -1,
                Name = fundType.Name,
                BankNumber = "42",
                Code = "123",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                FundType = fundType,
                Organization = organization
            };

            fund.Subsidiaries.Add(account);
            account.Fund = fund;


            context.Funds.Add(fund);

            context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.Delete();
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Receipt()
        {

            var receipt = new Receipt
            {
                Id = -1,
                Amount = 200,
                ReceiptType = receiptType,
                Organization = organization,
                ReceivedFrom = "test",
                Fund = fund,
                Account = account,
                CreatedBy = "Foo",
                UpdatedBy = "Foo"
            };

            account.Transactions.Add(receipt);

            context.SaveChanges();

            Assert.AreNotEqual(-1, receipt.Id);
        }
    }
}