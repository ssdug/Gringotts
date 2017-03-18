using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.ExpenseCategories;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.LivingUnits;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Wiz.Gringotts.UIWeb.Models.ReceiptSources;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public static class SeedData
    {
        public static void Execute(ApplicationDbContext context)
        {
            //initialize lookup lists
            AddClientIdentifierTypes(context);
            AddPayeeTypes(context);
            AddFeatures(context);
            AddAccountTypes(context);
            AddFundTypes(context);
            AddTransactionSubTypes(context);

            //populate default entities
            if (!context.Organizations.Any())
            {
                AddDepartment(context);
                AddDivisions(context);
                AddAdministrations(context);
                AddInstitutions(context);
            }

            AddLivingUnits(context);
            AddExpenseCategories(context);
            AddReceiptSources(context);
            AddSystemPayees(context);

            context.SaveChanges();
        }

       
        private static void AddClientIdentifierTypes(ApplicationDbContext context)
        {
            if (context.ClientIdentifierTypes.Any())
                return;

            context.ClientIdentifierTypes.AddOrUpdate(new ClientIdentifierType { Id = 1, Name = "Medicare" });
            context.ClientIdentifierTypes.AddOrUpdate(new ClientIdentifierType { Id = 2, Name = "Wizard Registry" });
            context.ClientIdentifierTypes.AddOrUpdate(new ClientIdentifierType { Id = 3, Name = "Broom License" });
            context.ClientIdentifierTypes.AddOrUpdate(new ClientIdentifierType { Id = 4, Name = "SSN" });

            context.SaveChanges();
        }

        private static void AddPayeeTypes(ApplicationDbContext context)
        {
            if (context.PayeeTypes.Any())
                return;

            context.PayeeTypes.AddOrUpdate(new PayeeType { Id = 1, Name = Payee.Guardian });
            context.PayeeTypes.AddOrUpdate(new PayeeType { Id = 2, Name = Payee.Attorney });
            context.PayeeTypes.AddOrUpdate(new PayeeType { Id = 3, Name = Payee.Court });
            context.PayeeTypes.AddOrUpdate(new PayeeType { Id = 4, Name = Payee.Facility });

            context.SaveChanges();
        }

        private static void AddFeatures(ApplicationDbContext context)
        {
            if (context.Features.Any())
                return;

            context.Features.AddOrUpdate(new Feature { Id = 1, Name = Feature.Clients, Description = "Allows Clients to be associated with this Organization." });
            context.Features.AddOrUpdate(new Feature { Id = 2, Name = Feature.Funds, Description = "Creates default Funds for this Organization." });
            context.Features.AddOrUpdate(new Feature { Id = 3, Name = Feature.Payees, Description = "Allows Payees to be associated with this Organization." });
            context.Features.AddOrUpdate(new Feature { Id = 4, Name = Feature.Restitution, Description = "Enables Restitution feature for this Organization." });

            context.SaveChanges();
        }

        private static void AddAccountTypes(ApplicationDbContext context)
        {
            if (context.AccountTypes.Any())
                return;

            context.AccountTypes.AddOrUpdate(new AccountType{ Id = 1, Name = AccountType.Checking, IsDefault = true });
            context.AccountTypes.AddOrUpdate(new AccountType{ Id = 2, Name = AccountType.Savings, IsDefault = true });
            context.AccountTypes.AddOrUpdate(new AccountType{ Id = 3, Name = AccountType.Restitution, IsDefault = false });

            context.SaveChanges();
        }

        private static void AddFundTypes(ApplicationDbContext context)
        {
            if (context.FundTypes.Any())
                return;

            context.FundTypes.AddOrUpdate(new FundType { Id = 1, Code = "200", Name = "Petty Cash" });
            context.FundTypes.AddOrUpdate(new FundType { Id = 2, Code = "300", Name = "Store" });
            context.FundTypes.AddOrUpdate(new FundType { Id = 3, Code = "100", Name = "Trust Fund" });
            context.FundTypes.AddOrUpdate(new FundType { Id = 4, Code = "400", Name = "Transmittal" });
            context.FundTypes.AddOrUpdate(new FundType { Id = 5, Code = "500", Name = "Betterment" });
        }

        private static void AddTransactionSubTypes(ApplicationDbContext context)
        {
            if (context.TransactionSubTypes.Any())
                return;

            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 1, Name = ReceiptType.Seed });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 2, Name = ReceiptType.Cash, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 3, Name = ReceiptType.Coin, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 4, Name = ReceiptType.Check, UserSelectable = true, IsDefault = true });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 5, Name = ReceiptType.Payroll, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 6, Name = ReceiptType.EFT, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 7, Name = ReceiptType.MoneyOrder, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ReceiptType { Id = 8, Name = ReceiptType.Transfer });

            context.TransactionSubTypes.AddOrUpdate(new ExpenseType { Id = 9, Name = ExpenseType.Cash, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ExpenseType { Id = 10, Name = ExpenseType.Card, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ExpenseType { Id = 11, Name = ExpenseType.Check, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ExpenseType { Id = 12, Name = ExpenseType.PurchaseOrder, UserSelectable = true });
            context.TransactionSubTypes.AddOrUpdate(new ExpenseType { Id = 13, Name = ExpenseType.Transfer });

            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 14, Name = VoidType.Duplicate, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 15, Name = VoidType.WrongAccount, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 16, Name = VoidType.WrongVendor, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 17, Name = VoidType.WrongAmount, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 18, Name = VoidType.SpoiledDamaged, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 19, Name = VoidType.LostStolen, UserSelectable = true});
            context.TransactionSubTypes.AddOrUpdate(new VoidType { Id = 20, Name = VoidType.Other, UserSelectable = true});

        }

        private static void AddDepartment(ApplicationDbContext context)
        {
            var department = new Organization
            {
                Id = 1,
                Name = "Gringotts",
                Abbreviation = "GRIN",
                Phone = "1-800-555-1212",
                AddressLine1 = "1 Gringotts Way",
                City = "London",
                State = "UK",
                PostalCode = "10000",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "billyp",
                GroupName = "Gringotts",
            };

            AddAudit(department);

            context.Organizations.AddOrUpdate(department);
            context.SaveChanges();
        }

        private static void AddDivisions(ApplicationDbContext context)
        {
            var department = context.Organizations.First(o => o.Abbreviation == "GRIN");
            var divisions = new[]
            {
                new Organization
                {
                    Id = 2,
                    Abbreviation = "MOM",
                    Name = "Ministry of Magic",
                    GroupName = "Ministry",
                    Phone = "360-555-1212",
                    AddressLine1 = "1 Whitehall",
                    City = "London",
                    State = "UK",
                    PostalCode = "10000",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = department
                },
            };

            AddAudit(divisions);

            context.Organizations.AddOrUpdate(divisions);
            context.SaveChanges();

        }

        private static void AddAdministrations(ApplicationDbContext context)
        {
            var administrations = new[]
            {
                new Organization
                {
                    Id = 4,
                    Abbreviation = "EDU",
                    Name = "Department of Education",
                    GroupName = "Education",
                    Phone = "360-555-1212",
                    AddressLine1 = "123 Street",
                    City = "London",
                    State = "UK",
                    PostalCode = "98501",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("MOM"))
                },
                new Organization
                {
                    Id = 5,
                    Abbreviation = "MLE",
                    Name = "Department of Magical Law Enforcement",
                    GroupName = "Enforcement",
                    Phone = "360-555-1212",
                    AddressLine1 = "123 Street",
                    City = "London",
                    State = "UK",
                    PostalCode = "98501",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("MOM"))
                },
            };

            AddAudit(administrations);

            context.Organizations.AddOrUpdate(administrations);
            context.SaveChanges();

        }

        private static void AddInstitutions(ApplicationDbContext context)
        {
            var institutions = new[]
            {
                //Testing Institutions
                new Organization
                {
                    Id = 7,
                    Abbreviation = "DEV",
                    Name = "Developers All",
                    GroupName = "Developers",
                    Phone = "360-555-1212",
                    AddressLine1 = "123 Street",
                    City = "Olympia",
                    State = "WA",
                    PostalCode = "98501",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("GRIN"))
                },
                new Organization
                {
                    Id = 9,
                    Abbreviation = "TEST",
                    Name = "Testers",
                    GroupName = "Test",
                    Phone = "360-555-1212",
                    AddressLine1 = "123 Street",
                    City = "Olympia",
                    State = "WA",
                    PostalCode = "98501",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("GRIN"))
                },
                new Organization
                {
                    Id = 9,
                    Abbreviation = "AZK",
                    Name = "Azkaban",
                    GroupName = "Azkaban",
                    Phone = "360-555-1212",
                    AddressLine1 = "123 Street",
                    City = "North Sea",
                    State = "UK",
                    PostalCode = "10000",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("MLE"))
                },
                new Organization
                {
                    Id = 9,
                    Abbreviation = "HOG",
                    Name = "Hogwarts",
                    GroupName = "Hogwarts",
                    Phone = "360-555-1212",
                    AddressLine1 = "Hogwarts Castle",
                    City = "Highlands",
                    State = "SC",
                    PostalCode = "10000",
                    FiscalContactSamAccountName = "sallyj",
                    ITConactSamAccountName = "billyp",
                    Parent = context.Organizations.First(d => d.Abbreviation.Equals("EDU"))
                },
            };

            AddAudit(institutions);

            context.Organizations.AddOrUpdate(institutions);
            context.SaveChanges();
        }

        private static void AddLivingUnits(ApplicationDbContext context)
        {
            if (context.LivingUnits.Any())
                return;

            var hog = context.Organizations.First(o => o.Abbreviation == "HOG");
            var dev = context.Organizations.First(o => o.Abbreviation == "DEV");
            var test = context.Organizations.First(o => o.Abbreviation == "TEST");



            var livingUnits = new[]
            {

                //For testing
                new LivingUnit { Id = 309, OrganizationId = hog.Id, Name = "Gryffindor"        , IsActive = true}, 
                new LivingUnit { Id = 310, OrganizationId = hog.Id, Name = "Hufflepuff"        , IsActive = true}, 
                new LivingUnit { Id = 311, OrganizationId = hog.Id, Name = "Ravenclaw"         , IsActive = true}, 
                new LivingUnit { Id = 312, OrganizationId = hog.Id, Name = "Slytherin"         , IsActive = true}, 

                //DEV
                new LivingUnit { Id = 301, OrganizationId = dev.Id, Name = "RED HOUSE"         , IsActive = true},
                new LivingUnit { Id = 302, OrganizationId = dev.Id, Name = "BLUE HOUSE"        , IsActive = true},
                new LivingUnit { Id = 303, OrganizationId = dev.Id, Name = "BLACK HOUSE"       , IsActive = true},

               
                 //Test
                new LivingUnit { Id = 304, OrganizationId = test.Id, Name = "TIMBER"           , IsActive = true},
                new LivingUnit { Id = 305, OrganizationId = test.Id, Name = "SOD"              , IsActive = true},
                new LivingUnit { Id = 306, OrganizationId = test.Id, Name = "STONE"            , IsActive = true},
                new LivingUnit { Id = 307, OrganizationId = test.Id, Name = "BRICK"            , IsActive = true},
                new LivingUnit { Id = 308, OrganizationId = test.Id, Name = "LOG"              , IsActive = true},

            };

            AddAudit(livingUnits);

            context.LivingUnits.AddOrUpdate(livingUnits);
            context.SaveChanges();
        }

        private static void AddExpenseCategories(ApplicationDbContext context)
        {
            if (context.ExpenseCategories.Any())
                return;

            var orgs = context.Organizations
                .ToArray();

            var categories = orgs.Select(org => new ExpenseCategory
            {
                Name = "None",
                OrganizationId = org.Id,
                Organization = org
            })
            .ToArray();

            AddAudit(categories);
            context.ExpenseCategories.AddOrUpdate(categories);
            context.SaveChanges();
        }

        private static void AddReceiptSources(ApplicationDbContext context)
        {
            if (context.ReceiptSources.Any())
                return;

            var orgs = context.Organizations
                .ToArray();

            var sources = orgs.Select(org => new ReceiptSource
            {
                Name = "None",
                OrganizationId = org.Id,
                Organization = org
            })
            .ToArray();

            AddAudit(sources);
            context.ReceiptSources.AddOrUpdate(sources);
            context.SaveChanges();
        }


        private static void AddSystemPayees(ApplicationDbContext context)
        {
            if (context.Payees.Any())
                return;

            var orgs = context.Organizations
                .ToArray();
            
            var systemPayees = orgs.Select(o => new Payee
                {
                    Name = "System",
                    AddressLine1 = o.AddressLine1,
                    AddressLine2 = o.AddressLine2,
                    City = o.City,
                    State = o.State,
                    PostalCode = o.PostalCode,
                    Phone = o.Phone,
                    OrganizationId = o.Id,
                    IsUserSelectable = false
                })
                .ToArray();

            AddAudit(systemPayees);
            context.Payees.AddOrUpdate(systemPayees);
            context.SaveChanges();
        }


        private static void AddAudit(params IAmAuditable[] auditables)
        {
            var now = DateTime.UtcNow;
            var by = "ORG\\billyp";

            auditables.ForEach(o =>
            {
                o.Created = o.Updated = now;
                o.CreatedBy = o.UpdatedBy = by;
            });
        }
    }
}