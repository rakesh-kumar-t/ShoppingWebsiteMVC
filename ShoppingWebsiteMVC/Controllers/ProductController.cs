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
        ShoppingContext db = new ShoppingContext();
        // GET : Products by Category
        [Authorize]
        [HttpGet]
        public ActionResult Index(string category)
        {
            if(category==null)
            {
                // returns the category view(Index view)
                return View(db.Products.ToList());
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
                    var products = db.Products.Where(p => p.CategoryName.Equals(category)).ToList();
                        
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
        public ActionResult Search(string ProductName)
        {
            List<Product> list = new List<Product>();
            if(String.IsNullOrEmpty(ProductName))
            {
                var query = from product in db.Products select new Product { ProductId = product.ProductId, ProductName = product.ProductName, CategoryName = product.CategoryName,BrandName=product.BrandName, Price = product.Price,Units=product.Units,Discount = product.Discount, SupplierName = product.SupplierName };
                list = query.ToList();

            }
            else
            {
                var query = from product in db.Products
                            where product.ProductName.ToLower().Contains(ProductName.ToLower())
                            select new Product { ProductId = product.ProductId, ProductName = product.ProductName, CategoryName = product.CategoryName,BrandName=product.BrandName, Price = product.Price, Units = product.Units, Discount = product.Discount, SupplierName = product.SupplierName };
                list = query.ToList();

            }
            ViewBag.Title= "Search Result";
            return View("Products", list);
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
                        cart.ProductName = p.ProductName;
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
                    Transaction Trx = new Transaction();
                    Bill newbill = new Bill();
                    var t=db.Bills.Select(tn => tn.BillNo).DefaultIfEmpty(0).Max()+1;
                    newbill.BillNo = t;
                    newbill.Amount = c.Amount;
                    db.Bills.Add(newbill);
                    db.SaveChanges();
                   
                    Trx.BillNo = t;
                    Trx.UserId = c.UserId;
                    Trx.ProductId = c.ProductId;
                    Trx.ProductName = c.ProductName;
                    Trx.NoofProduct = c.NoofProduct;
                    Trx.Amount = c.Amount;
                    Trx.TDate = DateTime.Now;
                    db.Transactions.Add(Trx);
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
            var t = db.Transactions.Select(tn => tn.BillNo).DefaultIfEmpty(0).Max()+1;
            Bill newbill = new Bill();
            newbill.BillNo = t;
            db.Bills.Add(newbill);
            db.SaveChanges();
            double sum = 0;
           
            for (int i=0;i<cart.Count;i++)
            {
                Transaction Trx = new Transaction();
                Trx.BillNo = t;
                Trx.UserId = cart[i].UserId;
                var c = cart[i].ProductId.ToString();
                Trx.ProductId = c;
                Trx.ProductName = cart[i].ProductName;
                Trx.NoofProduct = cart[i].NoofProduct;
                Trx.Amount = cart[i].Amount;
                sum = sum + Trx.Amount;
                Trx.TDate = DateTime.Now;
                db.Transactions.Add(Trx);
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


    }
}