using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Data
{
    [TestClass]
    public class ClientPersistenceTests
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
        public void Can_persist_Client()
        {
            var client = new Client
            {
                Id = -1,
                FirstName = "Millhouse",
                LastName = "Manastorm",
                UpdatedBy = "Foo",
                CreatedBy = "Foo"
            };

            client.AddResidency(organization);

            context.Clients.Add(client);
            context.SaveChanges();

            Assert.AreNotEqual(-1, client.Id);
        }


        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_a_ClientIdentifier_with_Client()
        {
            var identifierType = context.ClientIdentifierTypes.First();

            var client = new Client
            {
                Id = -1,
                FirstName = "Millhouse",
                LastName = "Manastorm",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                Identifiers = new List<ClientIdentifier>
                {
                    new ClientIdentifier
                    {

                        ClientIdentifierType = identifierType,
                        Value = "42",
                        CreatedBy = "Foo",
                        UpdatedBy = "Foo"
                    }
                }
            };

            client.AddResidency(organization);

            context.Clients.Add(client);
            context.SaveChanges();

            Assert.AreNotEqual(-1, client.Id);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Can_persist_multiple_ClientIdentifiers_with_Client()
        {
            var identifierType1 = context.ClientIdentifierTypes.First();
            var identifierType2 = context.ClientIdentifierTypes.First(i => i.Id != identifierType1.Id);
            var client = new Client
            {
                Id = -1,
                FirstName = "Millhouse",
                LastName = "Manastorm",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                Identifiers = new List<ClientIdentifier>
                {
                    new ClientIdentifier
                    {

                        ClientIdentifierType = identifierType1,
                        Value = "42",
                        CreatedBy = "Foo",
                        UpdatedBy = "Foo"
                    },
                    new ClientIdentifier
                    {

                        ClientIdentifierType = identifierType2,
                        Value = "42",
                        CreatedBy = "Foo",
                        UpdatedBy = "Foo"
                    }
                }
            };

            client.AddResidency(organization);

            context.Clients.Add(client);
            context.SaveChanges();

            Assert.AreNotEqual(-1, client.Id);
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException)), TestCategory("Manual")]
        public void ClientIdentifiers_should_be_unique()
        {
            var identifierType1 = context.ClientIdentifierTypes.First();

            var client = new Client
            {
                Id = -1,
                FirstName = "Millhouse",
                LastName = "Manastorm",
                CreatedBy = "Foo",
                UpdatedBy = "Foo",
                Identifiers = new List<ClientIdentifier>
                {
                    new ClientIdentifier
                    {

                        ClientIdentifierType = identifierType1,
                        Value = "42",
                        CreatedBy = "Foo",
                        UpdatedBy = "Foo"
                    },
                    new ClientIdentifier
                    {

                        ClientIdentifierType = identifierType1,
                        Value = "42",
                        CreatedBy = "Foo",
                        UpdatedBy = "Foo"
                    }
                }
            };

            client.AddResidency(organization);

            context.Clients.Add(client);
            context.SaveChanges();
        }

    }
}
