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
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TId { get; set; }
        public int BillNo { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int NoofProduct { get; set; }
        public double Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime TDate { get; set; }

        public virtual User user{get;set;}
        public virtual Product product { get; set; }
        public virtual Bill Bill { get; set; }
        
    }
}