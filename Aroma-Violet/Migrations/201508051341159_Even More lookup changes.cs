namespace Aroma_Violet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvenMorelookupchanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        ClientID = c.String(),
                        AddressTypeID = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Client_ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.AddressType", t => t.AddressTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Client", t => t.Client_ClientId)
                .Index(t => t.AddressTypeID)
                .Index(t => t.Client_ClientId);
            
            CreateTable(
                "dbo.AddressType",
                c => new
                    {
                        AddressTypeId = c.Int(nullable: false, identity: true),
                        AddressTypeName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AddressTypeId);
            
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Province",
                c => new
                    {
                        ProvinceId = c.Int(nullable: false, identity: true),
                        ProvinceName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProvinceId);
            
            CreateTable(
                "dbo.AddressLine",
                c => new
                    {
                        AddressLineId = c.Int(nullable: false, identity: true),
                        AddressLineText = c.String(),
                        AddressID = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AddressLineId)
                .ForeignKey("dbo.Address", t => t.AddressID, cascadeDelete: true)
                .Index(t => t.AddressID);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        ContactName = c.String(),
                        ContactTypeID = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("dbo.Client", t => t.ClientID, cascadeDelete: true)
                .ForeignKey("dbo.Contact Type", t => t.ContactTypeID, cascadeDelete: true)
                .Index(t => t.ClientID)
                .Index(t => t.ContactTypeID);
            
            CreateTable(
                "dbo.Contact Type",
                c => new
                    {
                        ContactTypeId = c.Int(nullable: false, identity: true),
                        ContactTypeName = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ContactTypeId);
            
            AddColumn("dbo.Client", "NickName", c => c.String());
            AddColumn("dbo.Client", "FullNames", c => c.String());
            AddColumn("dbo.Client", "HomeLanguageID", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "Employer", c => c.String());
            AddColumn("dbo.Client", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.Client", "SAResident", c => c.Boolean(nullable: false));
            AddColumn("dbo.Client", "PostalAddressID", c => c.Int());
            AddColumn("dbo.Client", "DeliveryAddressID", c => c.Int());
            AddColumn("dbo.Client", "ProvinceID", c => c.Int());
            AddColumn("dbo.Client", "CountryID", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "DeliveryAddress_AddressId", c => c.Int());
            AddColumn("dbo.Client", "HomeLanguage_LanguageId", c => c.Int());
            AddColumn("dbo.Client", "PostalAddress_AddressId", c => c.Int());
            CreateIndex("dbo.Client", "ProvinceID");
            CreateIndex("dbo.Client", "CountryID");
            CreateIndex("dbo.Client", "DeliveryAddress_AddressId");
            CreateIndex("dbo.Client", "HomeLanguage_LanguageId");
            CreateIndex("dbo.Client", "PostalAddress_AddressId");
            AddForeignKey("dbo.Client", "CountryID", "dbo.Country", "CountryId", cascadeDelete: true);
            AddForeignKey("dbo.Client", "DeliveryAddress_AddressId", "dbo.Address", "AddressId");
            AddForeignKey("dbo.Client", "HomeLanguage_LanguageId", "dbo.Language", "LanguageId");
            AddForeignKey("dbo.Client", "PostalAddress_AddressId", "dbo.Address", "AddressId");
            AddForeignKey("dbo.Client", "ProvinceID", "dbo.Province", "ProvinceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contact", "ContactTypeID", "dbo.Contact Type");
            DropForeignKey("dbo.Contact", "ClientID", "dbo.Client");
            DropForeignKey("dbo.AddressLine", "AddressID", "dbo.Address");
            DropForeignKey("dbo.Address", "Client_ClientId", "dbo.Client");
            DropForeignKey("dbo.Client", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.Client", "PostalAddress_AddressId", "dbo.Address");
            DropForeignKey("dbo.Client", "HomeLanguage_LanguageId", "dbo.Language");
            DropForeignKey("dbo.Client", "DeliveryAddress_AddressId", "dbo.Address");
            DropForeignKey("dbo.Client", "CountryID", "dbo.Country");
            DropForeignKey("dbo.Address", "AddressTypeID", "dbo.AddressType");
            DropIndex("dbo.Contact", new[] { "ContactTypeID" });
            DropIndex("dbo.Contact", new[] { "ClientID" });
            DropIndex("dbo.AddressLine", new[] { "AddressID" });
            DropIndex("dbo.Client", new[] { "PostalAddress_AddressId" });
            DropIndex("dbo.Client", new[] { "HomeLanguage_LanguageId" });
            DropIndex("dbo.Client", new[] { "DeliveryAddress_AddressId" });
            DropIndex("dbo.Client", new[] { "CountryID" });
            DropIndex("dbo.Client", new[] { "ProvinceID" });
            DropIndex("dbo.Address", new[] { "Client_ClientId" });
            DropIndex("dbo.Address", new[] { "AddressTypeID" });
            DropColumn("dbo.Client", "PostalAddress_AddressId");
            DropColumn("dbo.Client", "HomeLanguage_LanguageId");
            DropColumn("dbo.Client", "DeliveryAddress_AddressId");
            DropColumn("dbo.Client", "CountryID");
            DropColumn("dbo.Client", "ProvinceID");
            DropColumn("dbo.Client", "DeliveryAddressID");
            DropColumn("dbo.Client", "PostalAddressID");
            DropColumn("dbo.Client", "SAResident");
            DropColumn("dbo.Client", "DateOfBirth");
            DropColumn("dbo.Client", "Employer");
            DropColumn("dbo.Client", "HomeLanguageID");
            DropColumn("dbo.Client", "FullNames");
            DropColumn("dbo.Client", "NickName");
            DropTable("dbo.Contact Type");
            DropTable("dbo.Contact");
            DropTable("dbo.AddressLine");
            DropTable("dbo.Province");
            DropTable("dbo.Country");
            DropTable("dbo.AddressType");
            DropTable("dbo.Address");
        }
    }
}
