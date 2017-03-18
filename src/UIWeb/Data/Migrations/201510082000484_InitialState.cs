using System.Data.Entity.Migrations;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public partial class InitialState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        AccountId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        BankNumber = c.String(maxLength: 255),
                        Comments = c.String(),
                        FundId = c.Int(nullable: false),
                        AccountTypeId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, storeType: "money"),
                        Encumbered = c.Decimal(nullable: false, storeType: "money"),
                        Available = c.Decimal(nullable: false, storeType: "money"),
                        IsActive = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ResidencyId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("dbo.AccountTypes", t => t.AccountTypeId)
                .ForeignKey("dbo.Residencies", t => t.ResidencyId)
                .ForeignKey("dbo.Funds", t => t.FundId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.FundId)
                .Index(t => t.AccountTypeId)
                .Index(t => t.OrganizationId)
                .Index(t => t.ResidencyId);
            
            CreateTable(
                "dbo.AccountTypes",
                c => new
                    {
                        AccountTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccountTypeId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Funds",
                c => new
                    {
                        SubsidiaryFundId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Code = c.String(nullable: false, maxLength: 3),
                        BankNumber = c.String(maxLength: 255),
                        Comments = c.String(),
                        FundTypeId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, storeType: "money"),
                        Encumbered = c.Decimal(nullable: false, storeType: "money"),
                        Available = c.Decimal(nullable: false, storeType: "money"),
                        IsActive = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.SubsidiaryFundId)
                .ForeignKey("dbo.FundTypes", t => t.FundTypeId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.FundTypeId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.FundTypes",
                c => new
                    {
                        FundTypeId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 3),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.FundTypeId)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        GroupName = c.String(nullable: false, maxLength: 255),
                        Abbreviation = c.String(nullable: false, maxLength: 255),
                        Phone = c.String(maxLength: 128),
                        AddressLine1 = c.String(nullable: false, maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        City = c.String(nullable: false, maxLength: 255),
                        State = c.String(nullable: false, maxLength: 255),
                        PostalCode = c.String(nullable: false, maxLength: 255),
                        FiscalContactSamAccountName = c.String(nullable: false, maxLength: 255),
                        ITConactSamAccountName = c.String(nullable: false, maxLength: 255),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.OrganizationId)
                .ForeignKey("dbo.Organizations", t => t.Parent_Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.GroupName, unique: true)
                .Index(t => t.Abbreviation, unique: true)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        FeatureId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.FeatureId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        TransactionSubTypeId = c.Int(nullable: false),
                        FundId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        Memo = c.String(maxLength: 255),
                        PayeeId = c.Int(),
                        ReceivedFrom = c.String(maxLength: 255),
                        ReceivedFor = c.String(maxLength: 255),
                        ReceiptNumber = c.String(maxLength: 255),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Funds", t => t.FundId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.TransactionSubTypes", t => t.TransactionSubTypeId)
                .ForeignKey("dbo.Payees", t => t.PayeeId)
                .Index(t => t.TransactionSubTypeId)
                .Index(t => t.FundId)
                .Index(t => t.AccountId)
                .Index(t => t.OrganizationId)
                .Index(t => t.PayeeId);
            
            CreateTable(
                "dbo.TransactionSubTypes",
                c => new
                    {
                        TransactionSubTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        UserSelectable = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TransactionSubTypeId)
                .Index(t => t.Name);
            
            CreateTable(
                "dbo.Payees",
                c => new
                    {
                        PayeeId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 128),
                        AddressLine1 = c.String(nullable: false, maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        City = c.String(nullable: false, maxLength: 255),
                        State = c.String(nullable: false, maxLength: 255),
                        PostalCode = c.String(nullable: false, maxLength: 255),
                        OrganizationId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.PayeeId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.PayeeTypes",
                c => new
                    {
                        PayeeTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.PayeeTypeId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Residencies",
                c => new
                    {
                        ResidencyId = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        LivingUnitId = c.Int(),
                        OrganizationId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResidencyId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.LivingUnits", t => t.LivingUnitId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.ClientId)
                .Index(t => t.LivingUnitId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 128),
                        MiddleName = c.String(maxLength: 128),
                        LastName = c.String(maxLength: 128),
                        HasClientProperty = c.Boolean(nullable: false),
                        Comments = c.String(),
                        ImageId = c.Int(),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.ClientIdentifiers",
                c => new
                    {
                        ClientIdentifierId = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ClientIdentiferTypeId = c.Int(nullable: false),
                        Value = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ClientIdentifierId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.ClientIdentifierTypes", t => t.ClientIdentiferTypeId)
                .Index(t => new { t.ClientId, t.ClientIdentiferTypeId }, unique: true, name: "IX_ClientIdClientIdentifierTypeId");
            
            CreateTable(
                "dbo.ClientIdentifierTypes",
                c => new
                    {
                        ClientIdentifierTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ClientIdentifierTypeId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.LivingUnits",
                c => new
                    {
                        LivingUnitId = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.LivingUnitId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(maxLength: 128),
                        IsPropertyDamage = c.Boolean(nullable: false),
                        WithholdingPercent = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, storeType: "money"),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        Comments = c.String(),
                        IsSatified = c.Boolean(nullable: false),
                        SatisfiedReason = c.String(),
                        PayeeId = c.Int(nullable: false),
                        ResidencyId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Payees", t => t.PayeeId)
                .ForeignKey("dbo.Residencies", t => t.ResidencyId)
                .Index(t => t.PayeeId)
                .Index(t => t.ResidencyId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileId);
            
            CreateTable(
                "dbo.OrganizationFeatures",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.FeatureId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Features", t => t.FeatureId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.FeatureId);
            
            CreateTable(
                "dbo.PayeeTypePayees",
                c => new
                    {
                        PayeeType_Id = c.Int(nullable: false),
                        Payee_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PayeeType_Id, t.Payee_Id })
                .ForeignKey("dbo.PayeeTypes", t => t.PayeeType_Id, cascadeDelete: true)
                .ForeignKey("dbo.Payees", t => t.Payee_Id, cascadeDelete: true)
                .Index(t => t.PayeeType_Id)
                .Index(t => t.Payee_Id);
            
            CreateTable(
                "dbo.ResidencyAttorneys",
                c => new
                    {
                        ResidencyId = c.Int(nullable: false),
                        PayeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResidencyId, t.PayeeId })
                .ForeignKey("dbo.Residencies", t => t.ResidencyId, cascadeDelete: true)
                .ForeignKey("dbo.Payees", t => t.PayeeId, cascadeDelete: true)
                .Index(t => t.ResidencyId)
                .Index(t => t.PayeeId);
            
            CreateTable(
                "dbo.ResidencyGuardians",
                c => new
                    {
                        ResidencyId = c.Int(nullable: false),
                        PayeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResidencyId, t.PayeeId })
                .ForeignKey("dbo.Residencies", t => t.ResidencyId, cascadeDelete: true)
                .ForeignKey("dbo.Payees", t => t.PayeeId, cascadeDelete: true)
                .Index(t => t.ResidencyId)
                .Index(t => t.PayeeId);
            
            CreateTable(
                "dbo.FundsClientAccounts",
                c => new
                    {
                        FundId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FundId, t.AccountId })
                .ForeignKey("dbo.Funds", t => t.FundId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.FundId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.FundsSubsidiaryAccounts",
                c => new
                    {
                        FundId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FundId, t.AccountId })
                .ForeignKey("dbo.Funds", t => t.FundId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.FundId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Accounts", "FundId", "dbo.Funds");
            DropForeignKey("dbo.FundsSubsidiaryAccounts", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.FundsSubsidiaryAccounts", "FundId", "dbo.Funds");
            DropForeignKey("dbo.FundsClientAccounts", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.FundsClientAccounts", "FundId", "dbo.Funds");
            DropForeignKey("dbo.Residencies", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Orders", "ResidencyId", "dbo.Residencies");
            DropForeignKey("dbo.Orders", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.Orders", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Residencies", "LivingUnitId", "dbo.LivingUnits");
            DropForeignKey("dbo.LivingUnits", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.ResidencyGuardians", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.ResidencyGuardians", "ResidencyId", "dbo.Residencies");
            DropForeignKey("dbo.Residencies", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ClientIdentifiers", "ClientIdentiferTypeId", "dbo.ClientIdentifierTypes");
            DropForeignKey("dbo.ClientIdentifiers", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ResidencyAttorneys", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.ResidencyAttorneys", "ResidencyId", "dbo.Residencies");
            DropForeignKey("dbo.Accounts", "ResidencyId", "dbo.Residencies");
            DropForeignKey("dbo.Transactions", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.PayeeTypePayees", "Payee_Id", "dbo.Payees");
            DropForeignKey("dbo.PayeeTypePayees", "PayeeType_Id", "dbo.PayeeTypes");
            DropForeignKey("dbo.Payees", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Transactions", "TransactionSubTypeId", "dbo.TransactionSubTypes");
            DropForeignKey("dbo.Transactions", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Transactions", "FundId", "dbo.Funds");
            DropForeignKey("dbo.Transactions", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Funds", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.OrganizationFeatures", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "Parent_Id", "dbo.Organizations");
            DropForeignKey("dbo.Funds", "FundTypeId", "dbo.FundTypes");
            DropForeignKey("dbo.Accounts", "AccountTypeId", "dbo.AccountTypes");
            DropIndex("dbo.FundsSubsidiaryAccounts", new[] { "AccountId" });
            DropIndex("dbo.FundsSubsidiaryAccounts", new[] { "FundId" });
            DropIndex("dbo.FundsClientAccounts", new[] { "AccountId" });
            DropIndex("dbo.FundsClientAccounts", new[] { "FundId" });
            DropIndex("dbo.ResidencyGuardians", new[] { "PayeeId" });
            DropIndex("dbo.ResidencyGuardians", new[] { "ResidencyId" });
            DropIndex("dbo.ResidencyAttorneys", new[] { "PayeeId" });
            DropIndex("dbo.ResidencyAttorneys", new[] { "ResidencyId" });
            DropIndex("dbo.PayeeTypePayees", new[] { "Payee_Id" });
            DropIndex("dbo.PayeeTypePayees", new[] { "PayeeType_Id" });
            DropIndex("dbo.OrganizationFeatures", new[] { "FeatureId" });
            DropIndex("dbo.OrganizationFeatures", new[] { "OrganizationId" });
            DropIndex("dbo.Orders", new[] { "OrganizationId" });
            DropIndex("dbo.Orders", new[] { "ResidencyId" });
            DropIndex("dbo.Orders", new[] { "PayeeId" });
            DropIndex("dbo.LivingUnits", new[] { "OrganizationId" });
            DropIndex("dbo.ClientIdentifierTypes", new[] { "Name" });
            DropIndex("dbo.ClientIdentifiers", "IX_ClientIdClientIdentifierTypeId");
            DropIndex("dbo.Residencies", new[] { "OrganizationId" });
            DropIndex("dbo.Residencies", new[] { "LivingUnitId" });
            DropIndex("dbo.Residencies", new[] { "ClientId" });
            DropIndex("dbo.PayeeTypes", new[] { "Name" });
            DropIndex("dbo.Payees", new[] { "OrganizationId" });
            DropIndex("dbo.TransactionSubTypes", new[] { "Name" });
            DropIndex("dbo.Transactions", new[] { "PayeeId" });
            DropIndex("dbo.Transactions", new[] { "OrganizationId" });
            DropIndex("dbo.Transactions", new[] { "AccountId" });
            DropIndex("dbo.Transactions", new[] { "FundId" });
            DropIndex("dbo.Transactions", new[] { "TransactionSubTypeId" });
            DropIndex("dbo.Features", new[] { "Name" });
            DropIndex("dbo.Organizations", new[] { "Parent_Id" });
            DropIndex("dbo.Organizations", new[] { "Abbreviation" });
            DropIndex("dbo.Organizations", new[] { "GroupName" });
            DropIndex("dbo.Organizations", new[] { "Name" });
            DropIndex("dbo.FundTypes", new[] { "Name" });
            DropIndex("dbo.FundTypes", new[] { "Code" });
            DropIndex("dbo.Funds", new[] { "OrganizationId" });
            DropIndex("dbo.Funds", new[] { "FundTypeId" });
            DropIndex("dbo.AccountTypes", new[] { "Name" });
            DropIndex("dbo.Accounts", new[] { "ResidencyId" });
            DropIndex("dbo.Accounts", new[] { "OrganizationId" });
            DropIndex("dbo.Accounts", new[] { "AccountTypeId" });
            DropIndex("dbo.Accounts", new[] { "FundId" });
            DropTable("dbo.FundsSubsidiaryAccounts");
            DropTable("dbo.FundsClientAccounts");
            DropTable("dbo.ResidencyGuardians");
            DropTable("dbo.ResidencyAttorneys");
            DropTable("dbo.PayeeTypePayees");
            DropTable("dbo.OrganizationFeatures");
            DropTable("dbo.Files");
            DropTable("dbo.Orders");
            DropTable("dbo.LivingUnits");
            DropTable("dbo.ClientIdentifierTypes");
            DropTable("dbo.ClientIdentifiers");
            DropTable("dbo.Clients");
            DropTable("dbo.Residencies");
            DropTable("dbo.PayeeTypes");
            DropTable("dbo.Payees");
            DropTable("dbo.TransactionSubTypes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Features");
            DropTable("dbo.Organizations");
            DropTable("dbo.FundTypes");
            DropTable("dbo.Funds");
            DropTable("dbo.AccountTypes");
            DropTable("dbo.Accounts");
        }
    }
}
