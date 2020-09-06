using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ShoppingWebsiteMVC.Models
{
    public class Product
    {
        [Key]
        [Required]
        [Display(Name ="Product Code")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name ="Product Name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name ="Brand")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name ="Units")]
        public int Units { get; set; }
        [Display(Name ="Discount")]
        public double Discount { get; set; }
        [Required]
        [Display(Name ="Supplier Name")]
        public string SupplierName { get; set; }

    }
}