namespace WarehouseIO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageUrlInItemModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "ImageUrl");
        }
    }
}
