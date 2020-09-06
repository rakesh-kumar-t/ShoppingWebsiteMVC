using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

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