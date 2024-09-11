namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModelCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        Type = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        EstPrice = c.Double(nullable: false),
                        Amount = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        Location = c.String(maxLength: 100),
                        MaxCapacity = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transfers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MadeByUserId = c.String(maxLength: 128),
                        FromWarehouseId = c.Int(nullable: false),
                        ToWarehouseId = c.Int(nullable: false),
                        MadeOn = c.DateTime(nullable: false),
                        ClosedOn = c.DateTime(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouses", t => t.FromWarehouseId)
                .ForeignKey("dbo.AspNetUsers", t => t.MadeByUserId)
                .ForeignKey("dbo.Warehouses", t => t.ToWarehouseId)
                .Index(t => t.MadeByUserId)
                .Index(t => t.FromWarehouseId)
                .Index(t => t.ToWarehouseId);
            
            CreateTable(
                "dbo.MovingItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        TransferId = c.Int(),
                        ShipmentId = c.Int(),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .ForeignKey("dbo.Transfers", t => t.TransferId)
                .Index(t => t.ItemId)
                .Index(t => t.TransferId)
                .Index(t => t.ShipmentId);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromWarehouseId = c.Int(nullable: false),
                        ShippingTo = c.String(maxLength: 200),
                        MadeOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouses", t => t.FromWarehouseId, cascadeDelete: true)
                .Index(t => t.FromWarehouseId);
            
            CreateTable(
                "dbo.ApplicationUserWarehouses",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Warehouse_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Warehouse_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.Warehouse_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Warehouse_Id);
            
            AlterColumn("dbo.AspNetUsers", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.AspNetUsers", "ProfileImage", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.ApplicationUserWarehouses", "Warehouse_Id", "dbo.Warehouses");
            DropForeignKey("dbo.ApplicationUserWarehouses", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MovingItems", "TransferId", "dbo.Transfers");
            DropForeignKey("dbo.MovingItems", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.Shipments", "FromWarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.MovingItems", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Transfers", "ToWarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Transfers", "MadeByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transfers", "FromWarehouseId", "dbo.Warehouses");
            DropIndex("dbo.ApplicationUserWarehouses", new[] { "Warehouse_Id" });
            DropIndex("dbo.ApplicationUserWarehouses", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Shipments", new[] { "FromWarehouseId" });
            DropIndex("dbo.MovingItems", new[] { "ShipmentId" });
            DropIndex("dbo.MovingItems", new[] { "TransferId" });
            DropIndex("dbo.MovingItems", new[] { "ItemId" });
            DropIndex("dbo.Transfers", new[] { "ToWarehouseId" });
            DropIndex("dbo.Transfers", new[] { "FromWarehouseId" });
            DropIndex("dbo.Transfers", new[] { "MadeByUserId" });
            DropIndex("dbo.Items", new[] { "WarehouseId" });
            AlterColumn("dbo.AspNetUsers", "ProfileImage", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Name", c => c.String());
            DropTable("dbo.ApplicationUserWarehouses");
            DropTable("dbo.Shipments");
            DropTable("dbo.MovingItems");
            DropTable("dbo.Transfers");
            DropTable("dbo.Warehouses");
            DropTable("dbo.Items");
        }
    }
}
