namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedWarehouseModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Warehouses", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Warehouses", "Description", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Warehouses", "Location", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Warehouses", "Location", c => c.String(maxLength: 100));
            AlterColumn("dbo.Warehouses", "Description", c => c.String(maxLength: 200));
            AlterColumn("dbo.Warehouses", "Name", c => c.String(maxLength: 100));
        }
    }
}
