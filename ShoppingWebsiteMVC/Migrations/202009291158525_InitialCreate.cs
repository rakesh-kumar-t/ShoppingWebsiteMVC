namespace ShoppingWebsiteMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ProductId = c.String(maxLength: 128),
                        NoofProduct = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.String(nullable: false, maxLength: 128),
                        ProductName = c.String(nullable: false),
                        BrandName = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Units = c.Int(nullable: false),
                        Discount = c.Double(nullable: false),
                        SubCategoryId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SubCategoryId)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        FeedbackId = c.Int(nullable: false, identity: true),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        FeedBack = c.String(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.FeedbackId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Firstname = c.String(nullable: false),
                        Lastname = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 255),
                        Address = c.String(nullable: false),
                        ContactNumber = c.String(nullable: false, maxLength: 10),
                        City = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        Role = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ProductId = c.String(maxLength: 128),
                        NoofProduct = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        TDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Transactions", t => t.TId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.TId)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TId = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.SupplierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.Products", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "TId", "dbo.Transactions");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Feedbacks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Carts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropIndex("dbo.SubCategories", new[] { "CategoryId" });
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "TId" });
            DropIndex("dbo.Feedbacks", new[] { "UserId" });
            DropIndex("dbo.Feedbacks", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropIndex("dbo.Products", new[] { "SubCategoryId" });
            DropIndex("dbo.Carts", new[] { "ProductId" });
            DropIndex("dbo.Carts", new[] { "UserId" });
            DropTable("dbo.Suppliers");
            DropTable("dbo.Categories");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Transactions");
            DropTable("dbo.Orders");
            DropTable("dbo.Users");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
        }
    }
}
