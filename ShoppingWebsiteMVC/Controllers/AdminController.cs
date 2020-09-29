using ShoppingWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace ShoppingWebsiteMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Adminproduct
        ShoppingDbContext db = new ShoppingDbContext();
        [Authorize]
        public ActionResult Index()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                
                return View(db.Products.ToList());
            }
            else
                return RedirectToAction("Index", "Product");
        }
        //Get details of a product 
        [Authorize]
        public ActionResult Details(string ProductId)
        {
            if (Session["Role"].ToString() == "Admin")
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
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        //Get create view of new product
        [Authorize]
        public ActionResult Create()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                ViewBag.SubCategory = db.SubCategories.ToList();
                ViewBag.Supplier = db.Suppliers.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        //Post create new product
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,CategoryName,BrandName,Price,Units,Discount,SupplierName")] Product product,HttpPostedFileBase Proimage)
        {
            ViewBag.SubCategory = db.SubCategories.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                if (Proimage != null)
                {
                    if (Proimage.ContentLength > 0)
                    {
                        if(Path.GetExtension(Proimage.FileName).ToLower()==".jpg"
                            || Path.GetExtension(Proimage.FileName).ToLower() == ".jpeg"
                            || Path.GetExtension(Proimage.FileName).ToLower() == ".png")
                        {
                            var path = Path.Combine(Server.MapPath("~/Images/"), product.ProductId+".jpg");
                            Proimage.SaveAs(path);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            return View(product);
        }
        //Edit Product details
        [Authorize]
        public ActionResult Edit(string ProductId)
        {
            ViewBag.SubCategory = db.SubCategories.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            if (Session["Role"].ToString() == "Admin")
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
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        //Post edit product details
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,CategoryName,BrandName,Price,Units,Discount,SupplierName")] Product product)
        {
            ViewBag.SubCategory = db.SubCategories.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        //Get Delete product 
        [Authorize]
        public ActionResult Delete(string ProductId)
        {
            if (Session["Role"].ToString() == "Admin")
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
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        //Post Delete product
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string ProductId)
        {
            Product product = db.Products.Find(ProductId);
            product.Units = 0;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult Settings()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Settings", "User");
            }
        }
        //Get edit admin details
        [Authorize]
        public ActionResult AdminEdit()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                string username = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
                User model = new User();
                model.Firstname = user.Firstname;
                model.Lastname = user.Lastname;
                model.Address = user.Address;
                model.ContactNumber = user.ContactNumber;
                model.City = user.City;
                model.Country = user.Country;
                return View(model);
            }
            else
            {
                return RedirectToAction("Edit", "User");
            }
            
        }
        //Post admin edit details
        [HttpPost]
        [Authorize]
        public ActionResult AdminEdit(User usr)
        {
            
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            user.Firstname = usr.Firstname;
            user.Lastname = usr.Lastname;
            Session["Username"] = usr.Firstname + " " + usr.Lastname;
            user.Address = usr.Address;
            user.ContactNumber = usr.ContactNumber;
            user.City = usr.City;
            user.Country = usr.Country;
            user.Password = user.Password;
            user.ConfirmPassword = user.Password;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return View(usr);
            
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        //Post change password for user
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(User usr)
        {
            usr.Password =UserController.encrypt(usr.Password);
            usr.ConfirmPassword = UserController.encrypt(usr.ConfirmPassword);
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            user.Password = usr.Password;
            user.ConfirmPassword = usr.ConfirmPassword;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Settings");
        }
      
        [Authorize]
        public ActionResult Feedback()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                return View(db.Feedbacks.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }
        [Authorize]
        public ActionResult Report()
        {
            if (Session["Role"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("MyOrders", "User");
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult Report(string TDate)
        {
            DateTime dt;
            if(DateTime.TryParse(TDate,out dt ))
            {
                var trs = db.Orders.ToList();
                for(int i=0;i<trs.Count;i++)
                {
                    if (trs[i].TDate.Month != dt.Month)
                    {
                        trs.Remove(trs[i]);
                        i--;
                    }
                }
                return View("ReportView",trs);
            }
            else
            {
                return RedirectToAction("Report");
            }
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