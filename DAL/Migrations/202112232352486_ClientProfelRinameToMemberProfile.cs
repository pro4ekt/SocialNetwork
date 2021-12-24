namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientProfelRinameToMemberProfile : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ClientProfiles", newName: "MemberProfiles");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.MemberProfiles", newName: "ClientProfiles");
        }
    }
}
