namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleteTransferItemsInTransfer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MovingItems", "TransferId", "dbo.Transfers");
            DropIndex("dbo.MovingItems", new[] { "TransferId" });
            AlterColumn("dbo.MovingItems", "TransferId", c => c.Int(nullable: false));
            CreateIndex("dbo.MovingItems", "TransferId");
            AddForeignKey("dbo.MovingItems", "TransferId", "dbo.Transfers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovingItems", "TransferId", "dbo.Transfers");
            DropIndex("dbo.MovingItems", new[] { "TransferId" });
            AlterColumn("dbo.MovingItems", "TransferId", c => c.Int());
            CreateIndex("dbo.MovingItems", "TransferId");
            AddForeignKey("dbo.MovingItems", "TransferId", "dbo.Transfers", "Id");
        }
    }
}
