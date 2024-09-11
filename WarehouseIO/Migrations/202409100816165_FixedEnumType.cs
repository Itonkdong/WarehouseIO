namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedEnumType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "TypeString", c => c.String(maxLength: 50));
            DropColumn("dbo.Items", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Items", "TypeString");
        }
    }
}
