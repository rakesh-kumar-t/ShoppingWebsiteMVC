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
                var v = db.Users.Where(a => a.UserId.Equals(usr.UserId) && a.Password.Equals(usr.Password)).FirstOrDefault();
                if(v!=null)
                {
                    FormsAuthentication.SetAuthCookie(usr.UserId, false);
                    Session["UserId"] = v.UserId.ToString();
                    Session["Role"] = v.Role.ToString();
                    return RedirectToAction("Index");
                   
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials");
                }
                return View(usr);
            }

        }
    }
}