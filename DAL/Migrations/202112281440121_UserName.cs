namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ClientProfiles", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ClientProfiles", "Name", c => c.String());
        }
    }
}
