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
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public string ProductId { get; set; }
        public string productName { get; set; }
        public int NoofProduct { get; set; }
        public double Amount { get; set; }
    }
}