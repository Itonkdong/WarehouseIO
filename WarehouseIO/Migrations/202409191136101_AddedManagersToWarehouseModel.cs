namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedManagersToWarehouseModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WarehouseManagers",
                c => new
                    {
                        WarehouseId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.WarehouseId, t.UserId })
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.WarehouseId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WarehouseManagers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WarehouseManagers", "WarehouseId", "dbo.Warehouses");
            DropIndex("dbo.WarehouseManagers", new[] { "UserId" });
            DropIndex("dbo.WarehouseManagers", new[] { "WarehouseId" });
            DropTable("dbo.WarehouseManagers");
        }
    }
}
