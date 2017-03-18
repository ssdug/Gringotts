namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSelectableFlagToPayee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payees", "IsUserSelectable", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payees", "IsUserSelectable");
        }
    }
}
