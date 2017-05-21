namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingseasonidtogames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Game", "Season_Id", c => c.Int());
            CreateIndex("dbo.Game", "Season_Id");
            AddForeignKey("dbo.Game", "Season_Id", "dbo.Season", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Game", "Season_Id", "dbo.Season");
            DropIndex("dbo.Game", new[] { "Season_Id" });
            DropColumn("dbo.Game", "Season_Id");
        }
    }
}
