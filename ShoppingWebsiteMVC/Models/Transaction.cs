using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ShoppingWebsiteMVC.Models
{
    public class Transaction
    {
        [Key]
        public int TId { get; set; }
        public double Amount { get; set; }
        public virtual ICollection<Order> Orders{ get; set; }
    }
}