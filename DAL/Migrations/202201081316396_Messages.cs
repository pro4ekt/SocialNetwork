namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SenderId = c.String(maxLength: 128),
                        ReceiverId = c.String(),
                        Text = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientProfiles", t => t.SenderId)
                .Index(t => t.SenderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderId", "dbo.ClientProfiles");
            DropIndex("dbo.Messages", new[] { "SenderId" });
            DropTable("dbo.Messages");
        }
    }
}
