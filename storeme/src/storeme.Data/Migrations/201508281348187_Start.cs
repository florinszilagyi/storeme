namespace storeme.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DashboardItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        AddedOn = c.DateTime(nullable: false),
                        IsFolder = c.Boolean(nullable: false),
                        FileId = c.Int(),
                        Dashboard_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DashboardFiles", t => t.FileId, true)
                .ForeignKey("dbo.Dashboards", t => t.Dashboard_Id, true)
                .Index(t => t.FileId)
                .Index(t => t.Dashboard_Id);
            
            CreateTable(
                "dbo.DashboardFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MediaType = c.String(),
                        Content = c.Binary(),
                        Size = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dashboards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 128),
                        Salt = c.String(maxLength: 128),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DashboardItems", "Dashboard_Id", "dbo.Dashboards");
            DropForeignKey("dbo.DashboardItems", "FileId", "dbo.DashboardFiles");
            DropIndex("dbo.Dashboards", new[] { "Key" });
            DropIndex("dbo.DashboardItems", new[] { "Dashboard_Id" });
            DropIndex("dbo.DashboardItems", new[] { "FileId" });
            DropTable("dbo.Dashboards");
            DropTable("dbo.DashboardFiles");
            DropTable("dbo.DashboardItems");
        }
    }
}
