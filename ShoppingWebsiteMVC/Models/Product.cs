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
        [Required]
        [Display(Name ="ProductCode")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [Display(Name ="ProductName")]
        public string ProductName { get; set; }

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
        public int SubCategoryId { get; set; }
        public int SupplierId { get; set; }
        public double GetAmount(double price,double discount,int units)
        {
            double totamount = (price - (discount / 100) * price) * units;
            return totamount;
        }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual Supplier Supplier { get; set; }

    }
    
}