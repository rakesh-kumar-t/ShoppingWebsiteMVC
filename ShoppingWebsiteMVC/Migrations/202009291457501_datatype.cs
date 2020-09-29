namespace ShoppingWebsiteMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datatype : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Feedbacks", "ProductId", "dbo.Products");
            DropIndex("dbo.Carts", new[] { "ProductId" });
            DropIndex("dbo.Feedbacks", new[] { "ProductId" });
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Carts", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "ProductId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Feedbacks", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "ProductId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Products", "ProductId");
            CreateIndex("dbo.Carts", "ProductId");
            CreateIndex("dbo.Feedbacks", "ProductId");
            CreateIndex("dbo.Orders", "ProductId");
            AddForeignKey("dbo.Carts", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.Feedbacks", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropIndex("dbo.Feedbacks", new[] { "ProductId" });
            DropIndex("dbo.Carts", new[] { "ProductId" });
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Orders", "ProductId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Feedbacks", "ProductId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Products", "ProductId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Carts", "ProductId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Products", "ProductId");
            CreateIndex("dbo.Orders", "ProductId");
            CreateIndex("dbo.Feedbacks", "ProductId");
            CreateIndex("dbo.Carts", "ProductId");
            AddForeignKey("dbo.Feedbacks", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "ProductId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.Carts", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
