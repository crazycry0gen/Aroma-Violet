namespace Aroma_Violet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        ClientName = c.String(),
                        ClientSurname = c.String(),
                        IDNumber = c.String(),
                        ClientTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId)
                .ForeignKey("dbo.ClientType", t => t.ClientTypeID, cascadeDelete: true)
                .Index(t => t.ClientTypeID);
            
            CreateTable(
                "dbo.ClientType",
                c => new
                    {
                        ClientTypeId = c.Int(nullable: false, identity: true),
                        ClientTypeName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ClientTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Client", "ClientTypeID", "dbo.ClientType");
            DropIndex("dbo.Client", new[] { "ClientTypeID" });
            DropTable("dbo.ClientType");
            DropTable("dbo.Client");
        }
    }
}
