namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChecks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Checks",
                c => new
                    {
                        CheckId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        CheckNumber = c.Int(nullable: false),
                        PaidTo = c.String(),
                        PrintedBy = c.String(),
                        FundId = c.Int(nullable: false),
                        TransactionId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Transactions", t => new { t.TransactionId, t.AccountId })
                .ForeignKey("dbo.Funds", t => t.FundId)
                .Index(t => t.FundId)
                .Index(t => new { t.TransactionId, t.AccountId });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Checks", "FundId", "dbo.Funds");
            DropForeignKey("dbo.Checks", new[] { "TransactionId", "AccountId" }, "dbo.Transactions");
            DropIndex("dbo.Checks", new[] { "TransactionId", "AccountId" });
            DropIndex("dbo.Checks", new[] { "FundId" });
            DropTable("dbo.Checks");
        }
    }
}
