using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingWebsiteMVC.Models;


namespace ShoppingWebsiteMVC.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}