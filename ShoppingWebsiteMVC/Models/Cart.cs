using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingWebsiteMVC.Models
{
    public class Cart
    {
        [Key, Column(Order = 1)]
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }//Also a foreign key from user table
        [Key, Column(Order = 2)]
        public string ProductId { get; set; }//Also a foregin key from product table
        public string ProductName { get; set; }
        public int NoofProduct { get; set; }
        public double Amount { get; set; }
        
        [ForeignKey("UserId")]
        public User users{get;set;}
        [ForeignKey("ProductId")]
        public Product product { get; set; }
    }
}