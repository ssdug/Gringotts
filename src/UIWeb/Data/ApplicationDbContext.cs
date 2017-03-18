using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Wiz.Gringotts.UIWeb.Data.Conventions;
using Wiz.Gringotts.UIWeb.Data.Overrides;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.ExpenseCategories;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Files;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.LivingUnits;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Wiz.Gringotts.UIWeb.Models.ReceiptSources;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Wiz.Gringotts.UIWeb.Models.Checks;

namespace Wiz.Gringotts.UIWeb.Data
{

    /// <summary>
    /// The Entity Framework DbContext for the application.
    /// 
    /// <remarks>
    /// This type can be injected via dependency injection but may
    /// not have dependencies injected. This allows migration command
    /// line tools to work with out a dependency injection context.
    /// </remarks>
    /// </summary>
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            //insure default data is setup in the database
            Database.SetInitializer((IDatabaseInitializer<ApplicationDbContext>) new SeedDataInitializer());
            Configuration.LazyLoadingEnabled = true;
        }

        public ApplicationDbContext(DbConnection dbConnection): base(dbConnection, true)
        {
            //insure default data is setup in the database
            Database.SetInitializer((IDatabaseInitializer<ApplicationDbContext>) new SeedDataInitializer());
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new PrimaryKeyNamingConvention());
            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new AccountConfiguration());
            modelBuilder.Configurations.Add(new ResidencyConfiguration());
            modelBuilder.Configurations.Add(new OrganizationConfiguration());
            modelBuilder.Configurations.Add(new SubsidiaryFundConfiguration());
            modelBuilder.Configurations.Add(new ClientFundConfiguration());
            modelBuilder.Configurations.Add(new TransactionBatchConfiguration());
            modelBuilder.Configurations.Add(new TransactionConfiguration());
            modelBuilder.Configurations.Add(new TransactionSubTypeConfiguration());
            modelBuilder.Configurations.Add(new CheckConfiguration());
        }

        //Entities

        public virtual IDbSet<Organization> Organizations { get; set; }

        public virtual IDbSet<LivingUnit> LivingUnits { get; set; }

        public virtual IDbSet<Fund> Funds { get; set; }

        public virtual IDbSet<Account> Accounts { get; set; }

        public virtual IDbSet<TransactionBatch> Batches { get; set; }
        public virtual IDbSet<Transaction> Transactions { get; set; }

        public virtual IDbSet<Client> Clients { get; set; }

        public virtual IDbSet<Payee> Payees { get; set; }

        public virtual IDbSet<File> Files { get; set; }

        public virtual IDbSet<Order> Orders { get; set; }

        public virtual IDbSet<ClientIdentifier> ClientIdentifiers { get; set; }
        public virtual IDbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public virtual IDbSet<ReceiptSource> ReceiptSources { get; set; }

        //Lookups
        public virtual IDbSet<ClientIdentifierType> ClientIdentifierTypes { get; set; }

        public virtual IDbSet<PayeeType> PayeeTypes { get; set; }

        public virtual IDbSet<Feature> Features { get; set; }

        public virtual IDbSet<FundType> FundTypes { get; set; }

        public virtual IDbSet<AccountType> AccountTypes { get; set; }

        public virtual IDbSet<TransactionSubType> TransactionSubTypes { get; set; }

        public virtual IDbSet<Check> Checks { get; set; }

        void IUnitOfWork.BeginTransaction()
        {
            throw new NotImplementedException("Use TransactionCoordinatorDbContext");
        }

        void IUnitOfWork.CloseTransaction()
        {
            throw new NotImplementedException("Use TransactionCoordinatorDbContext");
        }

        void IUnitOfWork.CloseTransaction(Exception exception)
        {
            throw new NotImplementedException("Use TransactionCoordinatorDbContext");
        }
    }
}