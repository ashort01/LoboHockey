namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madeafewchanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Player_Id = c.Int(),
                        Season_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Player", t => t.Player_Id)
                .ForeignKey("dbo.Season", t => t.Season_Id)
                .Index(t => t.Player_Id)
                .Index(t => t.Season_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerRequest", "Season_Id", "dbo.Season");
            DropForeignKey("dbo.PlayerRequest", "Player_Id", "dbo.Player");
            DropIndex("dbo.PlayerRequest", new[] { "Season_Id" });
            DropIndex("dbo.PlayerRequest", new[] { "Player_Id" });
            DropTable("dbo.PlayerRequest");
        }
    }
}
