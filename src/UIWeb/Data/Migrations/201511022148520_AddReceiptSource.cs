namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReceiptSource : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReceiptSources",
                c => new
                    {
                        ReceiptSourceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        OrganizationId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Updated = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReceiptSourceId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => new { t.Name, t.OrganizationId }, unique: true, name: "IX_ReceiptSourceNameOrganizationId");
            
            AddColumn("dbo.Transactions", "ReceiptSourceId", c => c.Int());
            CreateIndex("dbo.Transactions", "ReceiptSourceId");
            AddForeignKey("dbo.Transactions", "ReceiptSourceId", "dbo.ReceiptSources", "ReceiptSourceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ReceiptSourceId", "dbo.ReceiptSources");
            DropForeignKey("dbo.ReceiptSources", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.ReceiptSources", "IX_ReceiptSourceNameOrganizationId");
            DropIndex("dbo.Transactions", new[] { "ReceiptSourceId" });
            DropColumn("dbo.Transactions", "ReceiptSourceId");
            DropTable("dbo.ReceiptSources");
        }
    }
}
