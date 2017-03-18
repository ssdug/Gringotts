namespace DSHS.WA.LocalFunds.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNaughtyFlagToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Residencies", "IsNaughty", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Residencies", "IsNaughty");
        }
    }
}
