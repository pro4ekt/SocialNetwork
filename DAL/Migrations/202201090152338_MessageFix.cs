namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "Id", "dbo.ClientProfiles");
            DropIndex("dbo.Messages", new[] { "Id" });
            DropPrimaryKey("dbo.Messages");
            AddColumn("dbo.Messages", "MessageId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Messages", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Messages", "ReceiverId", c => c.String());
            AlterColumn("dbo.Messages", "Text", c => c.String());
            AddPrimaryKey("dbo.Messages", "MessageId");
            CreateIndex("dbo.Messages", "Id");
            AddForeignKey("dbo.Messages", "Id", "dbo.ClientProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Id", "dbo.ClientProfiles");
            DropIndex("dbo.Messages", new[] { "Id" });
            DropPrimaryKey("dbo.Messages");
            AlterColumn("dbo.Messages", "Text", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Messages", "ReceiverId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Messages", "Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Messages", "MessageId");
            AddPrimaryKey("dbo.Messages", new[] { "Id", "ReceiverId", "Text", "DateTime" });
            CreateIndex("dbo.Messages", "Id");
            AddForeignKey("dbo.Messages", "Id", "dbo.ClientProfiles", "Id", cascadeDelete: true);
        }
    }
}
