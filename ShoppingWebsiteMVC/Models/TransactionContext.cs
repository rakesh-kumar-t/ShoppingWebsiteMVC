using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class TransactionContext:DbContext
    {
        public TransactionContext()
        {

        }
        public DbSet<Transaction> Transactions { get; set; }
    }
}