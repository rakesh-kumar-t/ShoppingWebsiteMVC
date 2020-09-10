using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ShoppingWebsiteMVC.Models
{
    public class UserContext:DbContext
    {
        public UserContext() : base("name=Dbconfig")
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }

}