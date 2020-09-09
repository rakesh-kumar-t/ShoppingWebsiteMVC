using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoppingWebsiteMVC.Models;
using System.Data.Entity;


namespace ShoppingWebsiteMVC.Controllers
{
    public class ProductController : Controller
    {
        
        // GET : Products by Category
        [Authorize]
        [HttpGet]
        public ActionResult Index(string category)
        {
            if(category==null)
            {
                // returns the category view(Index view)
                return View();
            }
            else
            {
                using(ProductContext db=new ProductContext())
                {
                    category = category.ToLower();
                    if (category == "all")
                    {
                        // return the View named Products with all products list
                        return View("Products",db.Products.ToList());
                    }
                    else
                    {
                        var products = db.Products.Where(p => p.CategoryName.Equals(category)).FirstOrDefault();
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
        }
        //Get Search product by productname
        public ActionResult Search(string ProductName)
        {
            List<Product> list = new List<Product>();
            using(ProductContext db=new ProductContext())
            {
                if(String.IsNullOrEmpty(ProductName))

                {
                    var query = from product in db.Products select new Product { ProductId = product.ProductId, ProductName = product.ProductName, CategoryName = product.CategoryName, Price = product.Price,Units=product.Units,Discount = product.Discount, SupplierName = product.SupplierName };
                    list = query.ToList();

                }
                else
                {
                    var query = from product in db.Products
                                where product.ProductName.ToLower().Contains(ProductName.ToLower())
                                select new Product { ProductId = product.ProductId, ProductName = product.ProductName, CategoryName = product.CategoryName, Price = product.Price, Units = product.Units, Discount = product.Discount, SupplierName = product.SupplierName };
                    list = query.ToList();

                }
                ViewBag.Title= "Search Result";
                return View("Products", list);
            }
        }
        //Get details of a product 
        [Authorize]
        public ActionResult ProductView(string ProductId)
        {
            using (ProductContext db = new ProductContext())
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
        }

     
        [Authorize]
        public ActionResult AddtoCart(string itemno,string ProductId)
        {
            using (ProductContext db = new ProductContext())
            {
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
                        using (CartContext dbo=new CartContext())
                        {
                            Cart cart = new Cart();
                            cart.UserId = Session["UserId"].ToString();
                            cart.ProductId = p.ProductId;
                            cart.ProductName = p.ProductName;
                            cart.Amount = p.GetAmount(p.Price,p.Discount,noofunits);
                            cart.NoofProduct = noofunits;
                            dbo.Carts.Add(cart);
                            dbo.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Cart","User");
            }
        }
        [Authorize]
        public ActionResult BuyNow(string UserId,string ProductId)
        {
            ViewBag.UserId = UserId;
            ViewBag.ProductId = ProductId;
            using (UserContext db = new UserContext())
            {
                var user=db.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();
                return View(user);
            }
        }
        [Authorize]
        public ActionResult ConfirmOrder(string UserId, string ProductId)
        {
            
            using (CartContext db = new CartContext())
            using (ProductContext dbo=new ProductContext())  
            {
                
                if (String.IsNullOrEmpty(ProductId))
                {
                    ViewBag.Error = "Empty";
                }
                else
                {
                    var c = db.Carts.Where(cart => cart.UserId.Equals(UserId)&&cart.ProductId.Equals(ProductId)).FirstOrDefault();

                    var p = dbo.Products.Where(product => product.ProductId.Equals(ProductId)).FirstOrDefault();
                    if (c.NoofProduct >p.Units )
                    {
                        ViewBag.Error = "No stock available";
                    }
                    else
                    {
                        using (TransactionContext dbc = new TransactionContext())
                        {
                            Transaction Trx = new Transaction();
                            Trx.UserId = c.UserId;
                            Trx.ProductId = c.ProductId;
                            Trx.NoofProduct = c.NoofProduct;
                            Trx.Amount = c.Amount;
                            Trx.TDate = DateTime.Now;
                            dbc.Transactions.Add(Trx);
                            dbc.SaveChanges();
                            p.Units = p.Units - c.NoofProduct;
                            dbo.Entry(p).State = EntityState.Modified;
                            dbo.SaveChanges();
                        }

                    }
                }
                return RedirectToAction("MyOrders","User");
            }
        }



    }
}