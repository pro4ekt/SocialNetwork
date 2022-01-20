namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientProfilceCountry_Add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientProfiles", "Country", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientProfiles", "Country");
        }
    }
}
