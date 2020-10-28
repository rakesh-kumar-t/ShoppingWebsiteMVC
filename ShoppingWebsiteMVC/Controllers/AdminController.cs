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
        DbShopContext db = new DbShopContext();
        [Authorize]
        public ActionResult Index()
        {
            if (Session["UserId"]!=null && Session["Role"].ToString() == "Admin")
            {
                if(TempData["status"]!=null && TempData["message"] != null)
                {
                    ViewBag.status = TempData["status"].ToString();
                    ViewBag.message = TempData["message"].ToString();
                    TempData["status"] = null;
                    TempData["message"] = null;
                }
                return View(db.Products.ToList());
            }
            else
                return RedirectToAction("Index", "Product");
        }
        //Get details of a product 
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (Session["UserId"]!=null && Session["Role"].ToString() == "Admin" && id!=null)
            {
                int ProductId = (int)id;
                Product product = db.Products.Find(ProductId);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        //Get create view of new product
        [Authorize]
        public ActionResult Create()
        {
            if (Session["UserId"]!=null&&Session["Role"].ToString() == "Admin")
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
        public ActionResult Create([Bind(Include = "ProductName,BrandName,Price,Units,Discount,SupplierId,SubCategoryId")] Product product,HttpPostedFileBase Proimage)
        {
            ViewBag.SubCategory = db.SubCategories.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["status"] = "success";
                TempData["message"] = product.ProductName + " added";
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
            else
            {
                ViewBag.status = "danger";
                ViewBag.message = "Please fill all fields and try again";
            }

            return View(product);
        }
        //Edit Product details
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id != null)
                {
                    int ProductId = (int)id;
                    ViewBag.SubCategory = db.SubCategories.ToList();
                    ViewBag.Supplier = db.Suppliers.ToList();
                    if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
                    {
                        Product product = db.Products.Find(ProductId);
                        if (product == null)
                        {
                            return HttpNotFound();
                        }
                        return View(product);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index","User");
            }
        }
        //Post edit product details
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,BrandName,Price,Units,Discount,SupplierId,SubCategoryId")] Product product, HttpPostedFileBase Proimage)
        {
            ViewBag.SubCategory = db.SubCategories.ToList();
            ViewBag.Supplier = db.Suppliers.ToList();
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                if (Proimage != null)
                {
                    if (Proimage.ContentLength > 0)
                    {
                        if (Path.GetExtension(Proimage.FileName).ToLower() == ".jpg"
                            || Path.GetExtension(Proimage.FileName).ToLower() == ".jpeg"
                            || Path.GetExtension(Proimage.FileName).ToLower() == ".png")
                        {
                            var path = Path.Combine(Server.MapPath("~/Images/"), product.ProductId + ".jpg");
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            Proimage.SaveAs(path);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(product);
        }
        //Get Delete product 
        [Authorize]
        public ActionResult Delete(int ProductId)
        {
            if (Session["Role"].ToString() == "Admin")
            {
                if (ProductId == 0)
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
        public ActionResult DeleteConfirmed(int ProductId)
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
            if (Session["UserId"]!=null && Session["Role"].ToString() == "Admin")
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
            if (Session["UserId"]!=null && Session["Role"].ToString() == "Admin")
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
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
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
                ViewBag.status = "success";
                ViewBag.message = "Details updated";
                return View(usr);
            }
            else
            {
                return RedirectToAction("Edit", "User");
            }
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View();
            }
            else
                return RedirectToAction("Index", "User");
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
            ModelState.Clear();
            ViewBag.status = "success";
            ViewBag.message = "Password changed";
            return View(usr);
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
        [Authorize]
        public ActionResult Suppliers()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View(db.Suppliers.ToList());
            }
            else
            {
                return RedirectToAction("Index","User");
            }
        }

        [Authorize]
        public ActionResult NewSupplier()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View();

            }
            else
            {
                return RedirectToAction("Index","User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSupplier([Bind(Include = "Name,Location")] Supplier supplier)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Suppliers.Add(supplier);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(supplier);
            }
            else
            {
                return RedirectToAction("Index","User");
            }
        }
        [Authorize]
        public ActionResult EditSupplier(int? id)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int SupplierId = (int)id;
                Supplier supplier = db.Suppliers.Find(SupplierId);
                if (supplier == null)
                {
                    return HttpNotFound();

                }
                return View(supplier);
            }
            else
            {
                return RedirectToAction("Index","User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSupplier([Bind(Include = "SupplierId,SupplierName,Location")] Supplier supplier)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                return View(supplier);
            }
            else
            {
                return RedirectToAction("Index","User");
            }
        }
        [Authorize]
        public ActionResult Categories()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View(db.Categories.ToList());
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult NewCategory()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View();

            }
            else
            {
                return RedirectToAction("Index","User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCategory([Bind(Include = "Name")] Category category)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult EditCategory(int? id)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();

                }
                return View(category);
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "CategoryId,Name")] Category category)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult SubCategories()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                return View(db.SubCategories.ToList());
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult NewSubCategory()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                ViewBag.Category = db.Categories.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSubCategory([Bind(Include = "Name,CategoryId")] SubCategory subcategory)
        {
            ViewBag.Category = db.Categories.ToList();
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.SubCategories.Add(subcategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(subcategory);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult EditSubCategory(int? id)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                ViewBag.Category = db.Categories.ToList();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }
                SubCategory subcategory = db.SubCategories.Find(id);
                if (subcategory == null)
                {
                    return HttpNotFound();

                }
                return View(subcategory);
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSubCategory([Bind(Include = "SubCategoryId,Name,CategoryId")] SubCategory subcategory)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "Admin")
            {
                ViewBag.Category = db.Categories.ToList();
                if (ModelState.IsValid)
                {
                    db.Entry(subcategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                return View(subcategory);
            }
            else
            {
                return RedirectToAction("Index", "User");
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