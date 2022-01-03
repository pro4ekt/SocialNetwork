namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BanProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientProfiles", "Banned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientProfiles", "Banned");
        }
    }
}
