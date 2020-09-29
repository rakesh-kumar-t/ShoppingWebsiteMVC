using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string FeedBack { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public virtual Product Products { get; set; }
        public virtual User Users { get; set; }
    }
}