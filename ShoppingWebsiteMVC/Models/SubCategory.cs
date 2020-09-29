using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class SubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }


        public virtual ICollection<Product> Products { get; set; }
        public virtual Category Category { get; set; }
    }
}