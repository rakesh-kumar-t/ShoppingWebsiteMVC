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
        public string ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string FeedBack { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public string TId { get; set; }

        [ForeignKey("ProductId")]
        public Product product { get; set; }
        [ForeignKey("UserId")]
        public User user { get; set; }
        [ForeignKey("TId")]
        public Transaction transaction { get; set; }
    }
}