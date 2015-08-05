namespace Aroma_Violet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lookupsadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EthnicGroup",
                c => new
                    {
                        EthnicGroupId = c.Int(nullable: false, identity: true),
                        EthnicGroupName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EthnicGroupId);
            
            CreateTable(
                "dbo.IncomeGroup",
                c => new
                    {
                        IncomeGroupId = c.Int(nullable: false, identity: true),
                        IncomeGroupName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IncomeGroupId);
            
            CreateTable(
                "dbo.Titel",
                c => new
                    {
                        TitelId = c.Int(nullable: false, identity: true),
                        TitelName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TitelId);
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        LanguageId = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LanguageId);
            
            AddColumn("dbo.Client", "TitelID", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "EthnicGroupID", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "IncomeGroupID", c => c.Int(nullable: false));
            CreateIndex("dbo.Client", "TitelID");
            CreateIndex("dbo.Client", "EthnicGroupID");
            CreateIndex("dbo.Client", "IncomeGroupID");
            AddForeignKey("dbo.Client", "EthnicGroupID", "dbo.EthnicGroup", "EthnicGroupId", cascadeDelete: true);
            AddForeignKey("dbo.Client", "IncomeGroupID", "dbo.IncomeGroup", "IncomeGroupId", cascadeDelete: true);
            AddForeignKey("dbo.Client", "TitelID", "dbo.Titel", "TitelId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Client", "TitelID", "dbo.Titel");
            DropForeignKey("dbo.Client", "IncomeGroupID", "dbo.IncomeGroup");
            DropForeignKey("dbo.Client", "EthnicGroupID", "dbo.EthnicGroup");
            DropIndex("dbo.Client", new[] { "IncomeGroupID" });
            DropIndex("dbo.Client", new[] { "EthnicGroupID" });
            DropIndex("dbo.Client", new[] { "TitelID" });
            DropColumn("dbo.Client", "IncomeGroupID");
            DropColumn("dbo.Client", "EthnicGroupID");
            DropColumn("dbo.Client", "TitelID");
            DropTable("dbo.Language");
            DropTable("dbo.Titel");
            DropTable("dbo.IncomeGroup");
            DropTable("dbo.EthnicGroup");
        }
    }
}
