using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace ShoppingWebsiteMVC.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TId { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int NoofProduct { get; set; }
        public double Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime TDate { get; set; }

        public virtual User Users{get;set;}
        public virtual Product Products { get; set; }
        public virtual Transaction Transactions { get; set; }
        
    }
}