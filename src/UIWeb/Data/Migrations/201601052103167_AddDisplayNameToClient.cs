namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisplayNameToClient : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE dbo.Clients ADD DisplayName AS LastName + ', ' + FirstName + ' ' + COALESCE(MiddleName,'')");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE dbo.Clients DROP COLUMN DisplayName");
        }
    }
}
