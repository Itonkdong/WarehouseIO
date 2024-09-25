namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedShipmentModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "FinalizedOn", c => c.DateTime());
            AddColumn("dbo.Shipments", "ShipmentStatusString", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "ShipmentStatusString");
            DropColumn("dbo.Shipments", "FinalizedOn");
        }
    }
}
