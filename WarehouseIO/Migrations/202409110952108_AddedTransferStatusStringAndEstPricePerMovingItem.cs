namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTransferStatusStringAndEstPricePerMovingItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transfers", "TransferStatusString", c => c.String(maxLength: 50));
            AddColumn("dbo.MovingItems", "EstPrice", c => c.Double(nullable: false));
            DropColumn("dbo.Transfers", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transfers", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.MovingItems", "EstPrice");
            DropColumn("dbo.Transfers", "TransferStatusString");
        }
    }
}
