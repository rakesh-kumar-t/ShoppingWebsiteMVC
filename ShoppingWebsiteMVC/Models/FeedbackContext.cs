using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingWebsiteMVC.Models
{
    public class FeedbackContext:DbContext
    {
        public FeedbackContext()
        {

        }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}