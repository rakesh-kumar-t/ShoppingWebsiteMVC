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
        // GET : Products by Category
        [Authorize]
        [HttpGet]
        public ActionResult Index(string category)
        {
            using(ProductContext db=new ProductContext())
            {
                category = category.ToLower();
                if (category == "all")
                {
                    return View(db.Products.ToList());
                }
                else
                {
                    var products = db.Products.Where(p => p.CategoryName.Equals(category)).FirstOrDefault();
                    if (products != null)
                    {
                        return View(products);
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Category";
                    }
                    return View("Index");
                }
            }
        }


    }
}