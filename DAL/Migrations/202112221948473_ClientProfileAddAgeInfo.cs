namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientProfileAddAgeInfo : DbMigration
    {
        public override void Up()
        {
            RenameTable("ClientProfile","ManagerProfile");
        }
        
        public override void Down()
        {
            RenameTable("ManagerProfile", "ClientProfile");
        }
    }
}
