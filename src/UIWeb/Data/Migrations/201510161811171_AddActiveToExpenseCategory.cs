using System.Data.Entity.Migrations;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public partial class AddActiveToExpenseCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCategories", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCategories", "IsActive");
        }
    }
}
