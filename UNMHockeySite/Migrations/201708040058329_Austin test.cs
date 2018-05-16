namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Austintest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Goal", "GameID", "dbo.Game");
            DropForeignKey("dbo.Game_Player", "Game_Id", "dbo.Game");
            AlterColumn("dbo.Game", "Id", c => c.Int(nullable: false, identity: true));
            AddForeignKey("dbo.Goal", "GameID", "dbo.Game", "Id");
            AddForeignKey("dbo.Game_Player", "Game_Id", "dbo.Game", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Game_Player", "Game_Id", "dbo.Game");
            DropForeignKey("dbo.Goal", "GameID", "dbo.Game");
            AlterColumn("dbo.Game", "Id", c => c.Int(nullable: false));
            AddForeignKey("dbo.Game_Player", "Game_Id", "dbo.Game", "Id");
            AddForeignKey("dbo.Goal", "GameID", "dbo.Game", "Id");
        }
    }
}
