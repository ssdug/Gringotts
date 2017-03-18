using System.Data.Entity.Migrations;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public partial class AddExpenseCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCategories",
                c => new
                    {
                        ExpenseCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        OrganizationId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExpenseCategoryId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => new { t.Name, t.OrganizationId }, unique: true, name: "IX_ExpenseCategoryNameOrganizationId");
            
            AddColumn("dbo.Transactions", "Effective", c => c.DateTime(nullable: false));
            AddColumn("dbo.Transactions", "Committed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "ExpenseCategoryId", c => c.Int());
            CreateIndex("dbo.Transactions", "ExpenseCategoryId");
            AddForeignKey("dbo.Transactions", "ExpenseCategoryId", "dbo.ExpenseCategories", "ExpenseCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ExpenseCategoryId", "dbo.ExpenseCategories");
            DropForeignKey("dbo.ExpenseCategories", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.ExpenseCategories", "IX_ExpenseCategoryNameOrganizationId");
            DropIndex("dbo.Transactions", new[] { "ExpenseCategoryId" });
            DropColumn("dbo.Transactions", "ExpenseCategoryId");
            DropColumn("dbo.Transactions", "Committed");
            DropColumn("dbo.Transactions", "Effective");
            DropTable("dbo.ExpenseCategories");
        }
    }
}
