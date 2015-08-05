namespace Aroma_Violet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class xx : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Contact Type", newName: "ContactType");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ContactType", newName: "Contact Type");
        }
    }
}
