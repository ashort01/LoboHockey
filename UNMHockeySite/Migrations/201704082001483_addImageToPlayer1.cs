namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addImageToPlayer1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Photos", "DateUploaded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "DateUploaded", c => c.DateTime(nullable: false));
        }
    }
}
