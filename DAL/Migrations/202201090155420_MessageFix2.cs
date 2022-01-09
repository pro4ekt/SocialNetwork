namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageFix2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Messages");
            DropColumn("dbo.Messages", "MessageId");
            AddColumn("dbo.Messages", "MessageId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Messages", "MessageId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Messages");
            DropColumn("dbo.Messages", "MessageId");
            AddColumn("dbo.Messages", "MessageId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Messages", "Id");
        }
    }
}
