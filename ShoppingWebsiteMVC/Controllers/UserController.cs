using ShoppingWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Data.Entity;

namespace ShoppingWebsiteMVC.Controllers
{
    public class UserController : Controller
    {
        ShoppingContext db = new ShoppingContext();
        // GET: User Home page and Login
        public ActionResult Index()
        {
            return View();
        }
        
        // Post User Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User usr )
        {
            usr.Password = encrypt(usr.Password);
            var user = db.Users.Where(a => a.UserId.Equals(usr.UserId) && a.Password.Equals(usr.Password)).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(usr.UserId, false);
                Session["UserId"] = user.UserId.ToString();
                Session["Username"] = (user.Firstname + " " + user.Lastname).ToString();
                Session["Role"] = user.Role.ToString();
                if(user.Role=="Admin")
                return RedirectToAction("Index", "Admin");
                else
                return RedirectToAction("Index", "Product");

            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials");
            }
                
            return View(usr);
        }
        //Get User Register Page
        public ActionResult Register()
        {
            return View();
        }
        
        //Post: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,Firstname,Lastname,Password,ConfirmPassword,Address,ContactNumber,City,Country")]User usr)
        {
            if(Session["Role"] != null && Session["Role"].Equals("Admin"))
            {
                usr.Role = "Admin";
            }
            else
            {
                usr.Role = "User";
            }
            
                usr.Password = encrypt(usr.Password);
                usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
                var check = db.Users.Find(usr.UserId);
                if(check==null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(usr);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                ModelState.AddModelError("", "User already Exists");
                return View();
                }
           
        }
        //User Logout action
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
        //Get Edit Current User info
        [Authorize]
        public ActionResult Edit()
        {
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            User model = new User();
            model.Firstname = user.Firstname;
            model.Lastname = user.Lastname;
            model.Address = user.Address;
            model.ContactNumber = user.ContactNumber;
            model.City=user.City;
            model.Country = user.Country;
            return View(model);

        }
        //Post Edit Current user info
        [Authorize]
        [HttpPost]
        public ActionResult Edit(User usr)
        {
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            user.Firstname = usr.Firstname;
            user.Lastname = usr.Lastname;
            user.Address = usr.Address;
            user.ContactNumber = usr.ContactNumber;
            user.City = usr.City;
            user.Country = usr.Country;
            user.Password = user.Password;
            user.ConfirmPassword = user.Password;
            Session["Username"] = (user.Firstname + " " + user.Lastname).ToString();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return View(usr);
        }
        //Encrypt password method
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        //Get change password for the user
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
            usr.Password = encrypt(usr.Password);
            usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            user.Password = usr.Password;
            user.ConfirmPassword = usr.ConfirmPassword;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Settings");
        }
        //Get cart page of the user
        [Authorize]
        [HttpGet]
        public ActionResult Cart()
        {
            string id = (string)Session["UserId"];
                
                var carts = db.Carts.Where(c => c.UserId.Equals(id)).ToList();
                if (carts != null)
                {
                    return View(carts);
                }
                else
                {
                    ViewBag.Error = "Cart Empty";
                }
                return View();
        }
        //Get id for deleting cart details of user
        [Authorize]
        public ActionResult CartDelete(string UserId,string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                
            Cart cart = db.Carts.Where(c => c.UserId.Equals(UserId)&&c.ProductId.Equals(ProductId)).FirstOrDefault();
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }
        //Post delete cart details
        [Authorize]
        [HttpPost, ActionName("CartDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UserId,string ProductId)
        {
            Cart cart = db.Carts.Where(c => c.UserId.Equals(UserId) && c.ProductId.Equals(ProductId)).FirstOrDefault();
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Cart");
        }
        //Get User Orders View
        [Authorize]
        public ActionResult MyOrders()
        {
            string UserId = Session["UserId"].ToString();
            var orders = db.Transactions.Where(c => c.UserId.Equals(UserId)).ToList().OrderBy(o=>o.TDate).ToList();
            return View(orders);
        }
        [Authorize]
        public ActionResult CancelOrder(int? TId)
        {
            if (TId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = db.Transactions.Where(t => t.TId==TId).FirstOrDefault();
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }
        [Authorize]
        [HttpPost, ActionName("CancelOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelallConfirmed(int TId,int BillNo)
        {
            var transaction = db.Transactions.Find(TId);
            double Amount = transaction.Amount;
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            var bill = db.Bills.Find(BillNo);
            bill.Amount = bill.Amount - Amount;
            db.Entry(bill).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyOrders");
        }
        [Authorize]
        public ActionResult Feedback(string ProductId)
        {
            ViewBag.ProductId = ProductId;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Feedback([Bind(Include = "Feedback")] Feedback fed,string ProductId)
        {
            fed.ProductId = ProductId;
            fed.UserId = Session["UserId"].ToString();
            db.Feedbacks.Add(fed);
            db.SaveChanges();
            return RedirectToAction("MyOrders");
        }
        [Authorize]
        public ActionResult Settings()
        {
            return View();
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