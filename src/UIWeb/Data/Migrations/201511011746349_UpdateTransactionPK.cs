namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionPK : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Transactions");
            AddPrimaryKey("dbo.Transactions", new[] { "TransactionId", "AccountId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Transactions");
            AddPrimaryKey("dbo.Transactions", "TransactionId");
        }
    }
}
