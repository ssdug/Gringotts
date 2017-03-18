using System.Linq;
using System.Runtime.CompilerServices;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Data
{
    [TestClass]
    public class FundPersistenceTests
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
        public void Can_persist_SubsidiaryFund()
        {
            var fundType = context.FundTypes.First();
            var fund = new SubsidiaryFund
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


            context.Funds.Add(fund);
            context.SaveChanges();

            Assert.AreNotEqual(-1, fund.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_SubsidiaryFund_with_a_SubsidiaryAccount()
        {
            var fundType = context.FundTypes.First();
            var fund = new SubsidiaryFund
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

            var accountType = context.AccountTypes.First();
            var subsidiaryAccount = new SubsidiaryAccount
            {
                Id = -1,
                Name = "SubsidiaryFund",
                BankNumber = "42",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                AccountType = accountType,
                Organization = organization
            };


            fund.Subsidiaries.Add(subsidiaryAccount);
            subsidiaryAccount.Fund = fund;

            context.Funds.Add(fund);
            context.SaveChanges();

            Assert.AreNotEqual(-1, fund.Id);
            Assert.AreNotEqual(-1, subsidiaryAccount.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_ClientFund()
        {
            var fundType = context.FundTypes.First();
            var fund = new ClientFund
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


            context.Funds.Add(fund);
            context.SaveChanges();

            Assert.AreNotEqual(-1, fund.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_ClientFund_with_a_ClientAccount()
        {
            var fundType = context.FundTypes.First();
            var fund = new ClientFund
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

            var clientAccountType = context.AccountTypes.First();
            var clientAccount = new ClientAccount
            {
                Id = -1,
                Name = "ClientFund",
                BankNumber = "42",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                AccountType = clientAccountType,
                Organization = organization
            };


            var client = new Client
            {
                Id = -1,
                FirstName = "Millhouse",
                LastName = "Manastorm",
                UpdatedBy = "Foo",
                CreatedBy = "Foo"
            };

            client.AddResidency(organization);

            clientAccount.Residency = client.Residencies.First();
            clientAccount.Fund = fund;
            fund.ClientAccounts.Add(clientAccount);


            context.Funds.Add(fund);
            context.SaveChanges();

            Assert.AreNotEqual(-1, fund.Id);
            Assert.AreNotEqual(-1, clientAccount.Id);
        }
    }
}