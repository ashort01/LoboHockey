namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addImageToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "PlayerImage_PhotoId", c => c.Int());
            CreateIndex("dbo.Player", "PlayerImage_PhotoId");
            AddForeignKey("dbo.Player", "PlayerImage_PhotoId", "dbo.Photos", "PhotoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "PlayerImage_PhotoId", "dbo.Photos");
            DropIndex("dbo.Player", new[] { "PlayerImage_PhotoId" });
            DropColumn("dbo.Player", "PlayerImage_PhotoId");
        }
    }
}
