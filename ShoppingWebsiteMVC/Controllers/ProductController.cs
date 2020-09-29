using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoppingWebsiteMVC.Models;
using System.Data.Entity;
using System.Web.DynamicData;

namespace ShoppingWebsiteMVC.Controllers
{
    public class ProductController : Controller
    {
        ShoppingDbContext db = new ShoppingDbContext();
        // GET : Products by Category
        [Authorize]
        [HttpGet]
        public ActionResult Index(string category)
        {
            if(category==null)
            {
                ViewBag.category = db.Categories.ToList();
                // returns the category view(Index view)
                //var cat = db.Products.GroupBy(m=>m.CategoryName).Select(y=>y.Count());
                return View();
            }
            else
            {
                if (category == "All")
                {
                    // return the View named Products with all products list
                    return View("Products",db.Products.ToList());
                }
                else
                {
                    var products = db.Products.Where(p => p.SubCategory.Category.Name.Equals(category)).ToList();
                        
                    if (products != null)
                    {
                        // return the View named Products with the required category lists 
                        return View("Products",products);
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Category";
                    }
                    // return the View named Products without any data 
                    return View("Products");
                }
            }
        }
        //Get Search product by productname
        [Authorize]
        public ActionResult Search(string ProductName)
        {
            var list = new List<Product>();
            if(String.IsNullOrEmpty(ProductName))
            {
                list = db.Products.ToList();
            }
            else
            {
                list = db.Products.Where(p => p.ProductName.ToLower().Equals(ProductName.ToLower())).ToList();
                if (list.Count == 0)
                {
                    list = db.Products.Where(p => p.SubCategory.Category.Name.ToLower().Equals(ProductName.ToLower())).ToList();
                    if (list.Count == 0)
                    {
                        list = db.Products.Where(p => p.BrandName.ToLower().Equals(ProductName.ToLower())).ToList();
                    }
                }
            }
            ViewBag.Title= "Search Result";
            return View("Products",list);
        }
        //Get details of a product 
        [Authorize]
        public ActionResult ProductView(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }
           
            return View(product);
        }

     
        [Authorize]
        [HttpPost]
        public ActionResult AddtoCart(string itemno,string ProductId)
        {
            string UserId =Session["UserId"].ToString();
            int noofunits = int.Parse(itemno);
            if (String.IsNullOrEmpty(ProductId))
            {
                ViewBag.Error = "Empty";
            }
            else
            {
                var p = db.Products.Where(pro => pro.ProductId.Equals(ProductId)).FirstOrDefault();

                if (noofunits > p.Units)
                {
                    ViewBag.Error = "No stock available";
                }
                else
                {
                    if (db.Carts.Where(car => car.ProductId.Equals(ProductId)&&car.UserId.Equals(UserId)).FirstOrDefault() != null)
                    {

                        Cart cart = db.Carts.Where(car => car.ProductId.Equals(ProductId) && car.UserId.Equals(UserId)).FirstOrDefault();
                        cart.NoofProduct= cart.NoofProduct+noofunits;
                        db.Entry(cart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        Cart cart = new Cart();
                        cart.UserId = Session["UserId"].ToString();
                        cart.ProductId = p.ProductId;
                        cart.NoofProduct = noofunits;
                        cart.Amount = p.GetAmount(p.Price, p.Discount, noofunits);
                        db.Carts.Add(cart);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Cart","User");
        }
        [Authorize]
        public ActionResult BuyNow(string UserId,string ProductId)
        {
            ViewBag.UserId = UserId;
            ViewBag.ProductId = ProductId;
            var user=db.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();
            return View(user);
        }
        [Authorize]
        public ActionResult ConfirmOrder(string UserId, string ProductId)
        {
            if (String.IsNullOrEmpty(ProductId))
            {
                ViewBag.Error = "Empty";
            }
            else
            {
                var c = db.Carts.Where(cart => cart.UserId.Equals(UserId)&&cart.ProductId.Equals(ProductId)).FirstOrDefault();

                var p = db.Products.Where(product => product.ProductId.Equals(ProductId)).FirstOrDefault();
                if (c.NoofProduct >p.Units )
                {
                    ViewBag.Error = "No stock available";
                }
                else
                {
                    Order Trx = new Order();
                    Transaction newbill = new Transaction();
                    var t=db.Orders.Select(tn => tn.TId).DefaultIfEmpty(0).Max()+1;
                    newbill.TId = t;
                    newbill.Amount = c.Amount;
                    db.Transactions.Add(newbill);
                    db.SaveChanges();
                   
                    Trx.TId = t;
                    Trx.UserId = c.UserId;
                    Trx.ProductId = c.ProductId;
                    Trx.NoofProduct = c.NoofProduct;
                    Trx.Amount = c.Amount;
                    Trx.TDate = DateTime.Now;
                    db.Orders.Add(Trx);
                    db.SaveChanges();
                    p.Units = p.Units - c.NoofProduct;
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Carts.Remove(c);
                    db.SaveChanges(); 
                }
            }
            return RedirectToAction("MyOrders","User");
        }
        [Authorize]
        public ActionResult BuyAll()
        {
            double sum = 0;
            string UserId = Session["UserId"].ToString();
            var cart = db.Carts.Where(c => c.UserId.Equals(UserId)).ToList();
            foreach(Cart c in cart)
            {
                sum = sum + c.Amount;
            }
            ViewBag.TotalSum = sum;
            var user = db.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();
            ViewBag.Address = user.Address+"\n"+user.City+"\n"+user.Country;
            ViewBag.Name = user.Firstname + " " + user.Lastname;
            return View(cart);
        }
        [Authorize]
        public ActionResult ConfirmAllOrder()
        {
            string UserId = Session["UserId"].ToString();
            var cart = db.Carts.Where(c => c.UserId.Equals(UserId)).ToList();
            var t = db.Transactions.Select(tn => tn.TId).DefaultIfEmpty(0).Max()+1;
            Transaction newbill = new Transaction();
            newbill.TId = t;
            db.Transactions.Add(newbill);
            db.SaveChanges();
            double sum = 0;
           
            for (int i=0;i<cart.Count;i++)
            {
                Order Trx = new Order();
                Trx.TId = t;
                Trx.UserId = cart[i].UserId;
                var c = cart[i].ProductId.ToString();
                Trx.ProductId = c;
                Trx.NoofProduct = cart[i].NoofProduct;
                Trx.Amount = cart[i].Amount;
                sum = sum + Trx.Amount;
                Trx.TDate = DateTime.Now;
                db.Orders.Add(Trx);
                db.SaveChanges();
                
                var p = db.Products.Where(pro => pro.ProductId.Equals(c)).FirstOrDefault();
                p.Units = p.Units - cart[i].NoofProduct;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                db.Carts.Remove(cart[i]);
                db.SaveChanges();
            }
            newbill.Amount = sum;
            db.Entry(newbill).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyOrders", "User");
        }
        //Dispose the database
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}