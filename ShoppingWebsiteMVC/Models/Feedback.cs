using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class Feedback
    {
        [Key]
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string FeedBack { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }

    }
}