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
        Models.DbShopContext db = new Models.DbShopContext();
        // GET : Products by Category
        [HttpGet]
        public ActionResult Index(string id)
        {
            string category = id;
            if(category==null)
            {
                return View(db.Categories.Where(p => p.Name != null).ToList());
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
                    var subcategory = db.SubCategories.Where(p => p.Category.Name.Equals(category)).ToList();
                        
                    if (subcategory != null)
                    {
                        // return the View named Products with the required category lists 
                        return View("SubCategory",subcategory);
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
        //View all the subcategories of a selected category
        public ActionResult SubCategory(string id)
        {
            string subcategory = id;
            if (subcategory != null)
            {
                var products = db.Products.Where(pro => pro.SubCategory.Name.Equals(subcategory)).ToList();
                if (products != null)
                {
                    return View("Products",products);
                }
                else
                {
                    ViewBag.Error = "Invalid Selection";
                }
                return View("Products");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        
        //Get Search product by productname
        public ActionResult Search(string id)
        {
            string ProductName = id;
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
        public ActionResult ProductView(int? id)
        {
            if (id != null)
            {
                int ProductId = (int)id;
                
                Product product = db.Products.Find(ProductId);
                if (product == null)
                {
                    return HttpNotFound();
                }
                if (TempData["status"] != null && TempData["Error"] != null)
                {
                    ViewBag.status = TempData["status"].ToString();
                    ViewBag.Error = TempData["Error"].ToString();
                    TempData["status"] = null;
                    TempData["Error"] = null;

                }
                return View(product);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
     
        [Authorize]
        [HttpPost]
        public ActionResult AddtoCart(string itemno,int? ProductId)
        {
            if(Session["UserId"]!=null && Session["Role"].ToString() == "User")
            {
                string UserId =Session["UserId"].ToString();
                int noofunits = int.Parse(itemno);
                if (ProductId==null)
                {
                    ViewBag.Error = "Empty";
                }
                else
                {
                    var p = db.Products.Find(ProductId);

                    if (noofunits > p.Units)
                    {
                        TempData["status"] = "danger";
                        TempData["Error"] = "No stock available";
                        return ProductView(id: ProductId);
                    }
                    else
                    {
                        if (db.Carts.Where(car => car.ProductId==ProductId && car.UserId.Equals(UserId)).FirstOrDefault() != null)
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
            else
            {
                return Redirect("/User/Index?ReturnUrl=/Product/ProductView/"+ProductId+"");
            }
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
        public ActionResult ConfirmOrder(string UserId, int? ProductId)
        {
            if (ProductId==null)
            {
                ViewBag.Error = "Empty";
            }
            else
            {
                var c = db.Carts.Where(cart => cart.UserId.Equals(UserId)&&cart.ProductId==ProductId).FirstOrDefault();

                var p = db.Products.Where(product => product.ProductId==ProductId).FirstOrDefault();
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
                var c = cart[i].ProductId;
                Trx.ProductId = c;
                Trx.NoofProduct = cart[i].NoofProduct;
                Trx.Amount = cart[i].Amount;
                sum = sum + Trx.Amount;
                Trx.TDate = DateTime.Now;
                db.Orders.Add(Trx);
                db.SaveChanges();
                
                var p = db.Products.Where(pro => pro.ProductId==c).FirstOrDefault();
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