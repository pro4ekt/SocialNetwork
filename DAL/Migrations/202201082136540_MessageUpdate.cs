namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageUpdate : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Messages");
            AddPrimaryKey("dbo.Messages", new[] { "Id", "ReceiverId", "Text", "DateTime" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Messages");
            AddPrimaryKey("dbo.Messages", new[] { "Id", "ReceiverId", "Text" });
        }
    }
}
