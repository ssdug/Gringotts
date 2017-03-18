namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParentChildrenToBatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionBatches", "TriggerId", c => c.Int());
            AddColumn("dbo.TransactionBatches", "TriggerAccountId", c => c.Int());
            AddColumn("dbo.TransactionBatches", "ParentId", c => c.Int());
            CreateIndex("dbo.TransactionBatches", new[] { "TriggerId", "TriggerAccountId" });
            CreateIndex("dbo.TransactionBatches", "ParentId");
            AddForeignKey("dbo.TransactionBatches", "ParentId", "dbo.TransactionBatches", "TransactionBatchId");
            AddForeignKey("dbo.TransactionBatches", new[] { "TriggerId", "TriggerAccountId" }, "dbo.Transactions", new[] { "TransactionId", "AccountId" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionBatches", new[] { "TriggerId", "TriggerAccountId" }, "dbo.Transactions");
            DropForeignKey("dbo.TransactionBatches", "ParentId", "dbo.TransactionBatches");
            DropIndex("dbo.TransactionBatches", new[] { "ParentId" });
            DropIndex("dbo.TransactionBatches", new[] { "TriggerId", "TriggerAccountId" });
            DropColumn("dbo.TransactionBatches", "ParentId");
            DropColumn("dbo.TransactionBatches", "TriggerAccountId");
            DropColumn("dbo.TransactionBatches", "TriggerId");
        }
    }
}
