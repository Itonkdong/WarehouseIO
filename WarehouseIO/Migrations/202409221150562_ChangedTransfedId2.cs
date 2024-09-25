namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTransfedId2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MovingItems", "ShipmentId", "dbo.Shipments");
            DropIndex("dbo.MovingItems", new[] { "TransferId" });
            AlterColumn("dbo.MovingItems", "TransferId", c => c.Int());
            CreateIndex("dbo.MovingItems", "TransferId");
            AddForeignKey("dbo.MovingItems", "ShipmentId", "dbo.Shipments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovingItems", "ShipmentId", "dbo.Shipments");
            DropIndex("dbo.MovingItems", new[] { "TransferId" });
            AlterColumn("dbo.MovingItems", "TransferId", c => c.Int(nullable: false));
            CreateIndex("dbo.MovingItems", "TransferId");
            AddForeignKey("dbo.MovingItems", "ShipmentId", "dbo.Shipments", "Id");
        }
    }
}
