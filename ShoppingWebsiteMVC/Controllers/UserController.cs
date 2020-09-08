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

namespace ShoppingWebsiteMVC.Controllers
{
    public class UserController : Controller
    {
        UserContext db = new UserContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User usr )
        {
            using(UserContext db=new UserContext())
            {
                usr.Password = encrypt(usr.Password);
                var user = db.Users.Where(a => a.UserId.Equals(usr.UserId) && a.Password.Equals(usr.Password)).FirstOrDefault();
                if(user!=null)
                {
                    FormsAuthentication.SetAuthCookie(usr.UserId, false);
                    Session["UserId"] = user.UserId.ToString();
                    Session["Username"] = (user.Firstname + " " + user.Lastname).ToString();
                    Session["Role"] = user.Role.ToString();
                    return RedirectToAction("Index");
                   
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials");
                }
              
            }
            return View(usr);


        }
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
            if(ModelState.IsValid)
            {
                usr.Password = encrypt(usr.Password);
                usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
                var check = db.Users.FirstOrDefault(a => a.UserId == usr.UserId);
                if(check==null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(usr);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exist";
                    return View();
                }
            }
            else
            {
                ViewBag.error = "Incomplete Data";
            }
            return View();
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
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
        
        
        [Authorize]
        [HttpGet]
        public ActionResult Cart()
        {
            using (CartContext db = new CartContext())
            {
               string id = (string)Session["UserId"];
                
                    var carts = db.Carts.Where(c => c.UserId.Equals(id)).FirstOrDefault();
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
        }
        [Authorize]
        public ActionResult CartDelete(string UserId,string ProductId)
        {
            using (CartContext db = new CartContext())
            {
                if (ProductId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Cart cart = db.Carts.Find(UserId, ProductId);
                if (cart == null)
                {
                    return HttpNotFound();
                }
                return View(cart);
            } 
        }
        [Authorize]
        [HttpPost, ActionName("CartDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UserId,string ProductId)
        {
            using (CartContext db = new CartContext())
            {
                Cart cart = db.Carts.Find(UserId,ProductId);
                db.Carts.Remove(cart);
                db.SaveChanges();
                return RedirectToAction("Cart");
            }
        }
    }
}