using System.Data.Entity.Migrations;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public partial class AddTransactionBatches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionBatches",
                c => new
                    {
                        TransactionBatchId = c.Int(nullable: false, identity: true),
                        BatchReferenceNumber = c.String(),
                        ExpectedAmount = c.Decimal(nullable: false, storeType: "money"),
                        Committed = c.Boolean(nullable: false),
                        Effective = c.DateTime(nullable: false),
                        TransactionSubTypeId = c.Int(),
                        FundId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        Memo = c.String(maxLength: 255),
                        PayeeId = c.Int(),
                        ExpenseCategoryId = c.Int(),
                        FromAccountId = c.Int(),
                        ToAccountId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TransactionBatchId)
                .ForeignKey("dbo.Funds", t => t.FundId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.TransactionSubTypes", t => t.TransactionSubTypeId)
                .ForeignKey("dbo.ExpenseCategories", t => t.ExpenseCategoryId)
                .ForeignKey("dbo.Payees", t => t.PayeeId)
                .ForeignKey("dbo.Accounts", t => t.FromAccountId)
                .ForeignKey("dbo.Accounts", t => t.ToAccountId)
                .Index(t => t.TransactionSubTypeId)
                .Index(t => t.FundId)
                .Index(t => t.OrganizationId)
                .Index(t => t.PayeeId)
                .Index(t => t.ExpenseCategoryId)
                .Index(t => t.FromAccountId)
                .Index(t => t.ToAccountId);
            
            AddColumn("dbo.Transactions", "BatchReferenceNumber", c => c.String());
            AddColumn("dbo.Transactions", "TransactionBatchId", c => c.Int());
            CreateIndex("dbo.Transactions", "TransactionBatchId");
            AddForeignKey("dbo.Transactions", "TransactionBatchId", "dbo.TransactionBatches", "TransactionBatchId");
            DropColumn("dbo.Transactions", "Committed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "Committed", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.TransactionBatches", "ToAccountId", "dbo.Accounts");
            DropForeignKey("dbo.TransactionBatches", "FromAccountId", "dbo.Accounts");
            DropForeignKey("dbo.TransactionBatches", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.TransactionBatches", "ExpenseCategoryId", "dbo.ExpenseCategories");
            DropForeignKey("dbo.TransactionBatches", "TransactionSubTypeId", "dbo.TransactionSubTypes");
            DropForeignKey("dbo.Transactions", "TransactionBatchId", "dbo.TransactionBatches");
            DropForeignKey("dbo.TransactionBatches", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.TransactionBatches", "FundId", "dbo.Funds");
            DropIndex("dbo.TransactionBatches", new[] { "ToAccountId" });
            DropIndex("dbo.TransactionBatches", new[] { "FromAccountId" });
            DropIndex("dbo.TransactionBatches", new[] { "ExpenseCategoryId" });
            DropIndex("dbo.TransactionBatches", new[] { "PayeeId" });
            DropIndex("dbo.TransactionBatches", new[] { "OrganizationId" });
            DropIndex("dbo.TransactionBatches", new[] { "FundId" });
            DropIndex("dbo.TransactionBatches", new[] { "TransactionSubTypeId" });
            DropIndex("dbo.Transactions", new[] { "TransactionBatchId" });
            DropColumn("dbo.Transactions", "TransactionBatchId");
            DropColumn("dbo.Transactions", "BatchReferenceNumber");
            DropTable("dbo.TransactionBatches");
        }
    }
}
