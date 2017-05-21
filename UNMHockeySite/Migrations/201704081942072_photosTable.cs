namespace UNMHockeySite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photosTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        PhotoId = c.Int(nullable: false, identity: true),
                        ImageData = c.Binary(storeType: "image"),
                        DateUploaded = c.DateTime(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PhotoId);
            
            DropColumn("dbo.Player", "Picture");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Player", "Picture", c => c.Binary(storeType: "image"));
            DropTable("dbo.Photos");
        }
    }
}
