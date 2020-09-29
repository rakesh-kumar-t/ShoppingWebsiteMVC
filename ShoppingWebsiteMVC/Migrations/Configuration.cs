namespace ShoppingWebsiteMVC.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShoppingWebsiteMVC.Models.ShoppingBagContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ShoppingWebsiteMVC.Models.ShoppingBagContext";
        }

        protected override void Seed(ShoppingWebsiteMVC.Models.ShoppingBagContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
