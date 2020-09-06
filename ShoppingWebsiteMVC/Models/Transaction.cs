using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ShoppingWebsiteMVC.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TId { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int NoofProduct { get; set; }
        public double Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime TDate { get; set; }

    }
}