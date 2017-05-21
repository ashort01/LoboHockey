namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingtablesseasonplayerandgameplayer1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Game_Player",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Game_Id = c.Int(),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Game", t => t.Game_Id)
                .ForeignKey("dbo.Player", t => t.Player_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Player_Id);
            
            CreateTable(
                "dbo.Season_Player",
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
            DropForeignKey("dbo.Season_Player", "Season_Id", "dbo.Season");
            DropForeignKey("dbo.Season_Player", "Player_Id", "dbo.Player");
            DropForeignKey("dbo.Game_Player", "Player_Id", "dbo.Player");
            DropForeignKey("dbo.Game_Player", "Game_Id", "dbo.Game");
            DropIndex("dbo.Season_Player", new[] { "Season_Id" });
            DropIndex("dbo.Season_Player", new[] { "Player_Id" });
            DropIndex("dbo.Game_Player", new[] { "Player_Id" });
            DropIndex("dbo.Game_Player", new[] { "Game_Id" });
            DropTable("dbo.Season_Player");
            DropTable("dbo.Game_Player");
        }
    }
}
