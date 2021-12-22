namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientProfileAddAgeInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientProfiles", "Age", c => c.Int(nullable: false));
            AddColumn("dbo.ClientProfiles", "Info", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientProfiles", "Info");
            DropColumn("dbo.ClientProfiles", "Age");
        }
    }
}
