using System.Data.Entity.Migrations;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public partial class AddCommentsToTransaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransactionBatches", "ExpenseCategoryId", "dbo.ExpenseCategories");
            DropIndex("dbo.TransactionBatches", new[] { "ExpenseCategoryId" });
            AddColumn("dbo.Transactions", "Comments", c => c.String());
            DropColumn("dbo.TransactionBatches", "ExpenseCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransactionBatches", "ExpenseCategoryId", c => c.Int());
            DropColumn("dbo.Transactions", "Comments");
            CreateIndex("dbo.TransactionBatches", "ExpenseCategoryId");
            AddForeignKey("dbo.TransactionBatches", "ExpenseCategoryId", "dbo.ExpenseCategories", "ExpenseCategoryId");
        }
    }
}
