namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransBatchAndMemoToCheck : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Checks", new[] { "TransactionId", "AccountId" });
            AddColumn("dbo.Checks", "Memo", c => c.String());
            AddColumn("dbo.Checks", "TransactionBatchId", c => c.Int());
            AlterColumn("dbo.Checks", "TransactionId", c => c.Int());
            AlterColumn("dbo.Checks", "AccountId", c => c.Int());
            CreateIndex("dbo.Checks", "AccountId");
            CreateIndex("dbo.Checks", new[] { "TransactionId", "AccountId" });
            CreateIndex("dbo.Checks", "TransactionBatchId");
            AddForeignKey("dbo.Checks", "AccountId", "dbo.Accounts", "AccountId");
            AddForeignKey("dbo.Checks", "TransactionBatchId", "dbo.TransactionBatches", "TransactionBatchId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Checks", "TransactionBatchId", "dbo.TransactionBatches");
            DropForeignKey("dbo.Checks", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Checks", new[] { "TransactionBatchId" });
            DropIndex("dbo.Checks", new[] { "TransactionId", "AccountId" });
            DropIndex("dbo.Checks", new[] { "AccountId" });
            AlterColumn("dbo.Checks", "AccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.Checks", "TransactionId", c => c.Int(nullable: false));
            DropColumn("dbo.Checks", "TransactionBatchId");
            DropColumn("dbo.Checks", "Memo");
            CreateIndex("dbo.Checks", new[] { "TransactionId", "AccountId" });
        }
    }
}
