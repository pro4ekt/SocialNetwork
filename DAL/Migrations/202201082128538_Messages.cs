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
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        Text = c.String(nullable: false, maxLength: 128),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.ReceiverId, t.Text })
                .ForeignKey("dbo.ClientProfiles", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Id", "dbo.ClientProfiles");
            DropIndex("dbo.Messages", new[] { "Id" });
            DropTable("dbo.Messages");
        }
    }
}
