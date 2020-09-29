using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingWebsiteMVC.Models
{
    public class Product
    {
        [Key]
        [Required]
        [Display(Name ="ProductCode")]
        public string ProductId { get; set; }
        [Required]
        [Display(Name ="ProductName")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name ="Category")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name ="Brand")]
        public string BrandName { get; set; }
        [Required]
        [Display(Name ="Cost")]
        public double Price { get; set; }
        [Required]
        [Display(Name ="Units")]
        public int Units { get; set; }
        [Display(Name ="Discount")]
        public double Discount { get; set; }
        [Required]
        [Display(Name ="SupplierName")]
        public string SupplierName { get; set; }
        public double GetAmount(double price,double discount,int units)
        {
            double totamount = (price - (discount / 100) * price) * units;
            return totamount;
        }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }

    }
    public enum Category
    {
        Laptops,
        SmartPhones,
        Clothing,
        Kitchen_Appliances,
        Footwear,
        Furniture,
        Fashion,
        Utilities,
        Jewellery,
        Electricals,
        Electronics,
        Wearables,
        Refrigerators,
        Home,
        ToysBaby,
        Sports,
        Appliances,
        Other
    }
}