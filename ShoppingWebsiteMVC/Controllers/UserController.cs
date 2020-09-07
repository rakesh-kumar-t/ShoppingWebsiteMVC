using ShoppingWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

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
    }
}