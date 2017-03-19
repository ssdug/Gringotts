using System.Linq;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Data
{
    [TestClass]
    public class ExpensePersistenceTests
    {
        private ApplicationDbContext context;
        private Organization organization;
        private SubsidiaryFund fund;
        private SubsidiaryAccount account;
        private Payee payee;
        private ExpenseType expenseType;

        [TestInitialize]
        public void Init()
        {
            context = TestDbContextFactory.Build();
            organization = context.Organizations.First();
            expenseType = context.TransactionSubTypes.OfType<ExpenseType>().First();

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

            payee = new Payee
            {
                Name = "Test",
                AddressLine1 = "123 Street",
                City = "Foo",
                State = "FO",
                PostalCode = "98503",
                Organization = organization,
                UpdatedBy = "Foo",
                CreatedBy = "Foo"
            };

            context.Payees.Add(payee);

            context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.Delete();
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_Expsense()
        {

            var expense = new Expense
            {
                Id = -1,
                Amount = 200,
                ExpenseType = expenseType,
                Memo = "Memo",
                Organization = organization,
                Fund = fund,
                Account = account,
                Payee = payee,
                CreatedBy = "Foo",
                UpdatedBy = "Foo"
            };

            account.Transactions.Add(expense);

            context.SaveChanges();

            Assert.AreNotEqual(-1, expense.Id);
        }
    }
}