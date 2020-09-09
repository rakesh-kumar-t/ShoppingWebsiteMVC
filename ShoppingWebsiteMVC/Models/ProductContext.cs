using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class ProductContext:DbContext
    {
        public ProductContext() : base("name=DBconfig")
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}