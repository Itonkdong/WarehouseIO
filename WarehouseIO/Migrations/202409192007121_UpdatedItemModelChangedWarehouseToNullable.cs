namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedItemModelChangedWarehouseToNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Items", "WarehouseId", "dbo.Warehouses");
            DropIndex("dbo.Items", new[] { "WarehouseId" });
            AlterColumn("dbo.Items", "WarehouseId", c => c.Int());
            CreateIndex("dbo.Items", "WarehouseId");
            AddForeignKey("dbo.Items", "WarehouseId", "dbo.Warehouses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "WarehouseId", "dbo.Warehouses");
            DropIndex("dbo.Items", new[] { "WarehouseId" });
            AlterColumn("dbo.Items", "WarehouseId", c => c.Int(nullable: false));
            CreateIndex("dbo.Items", "WarehouseId");
            AddForeignKey("dbo.Items", "WarehouseId", "dbo.Warehouses", "Id", cascadeDelete: true);
        }
    }
}
