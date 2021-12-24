namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientProfileAddAgeInfo : DbMigration
    {
        public override void Up()
        {
            RenameTable("MemberProfile","ManagerProfile");
        }
        
        public override void Down()
        {
            RenameTable("ManagerProfile", "MemberProfile");
        }
    }
}
