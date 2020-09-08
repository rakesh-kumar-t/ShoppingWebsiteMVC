using ShoppingWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace ShoppingWebsiteMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Adminproduct
        ProductContext dbo = new ProductContext();
        [Authorize]
        public ActionResult Index()
        {
            return View(dbo.Products.ToList());
        }
        [Authorize]
        public ActionResult Details(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = dbo.Products.Find(ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,CategoryName,Price,Discount,SupplierName")] Product product)
        {
            if (ModelState.IsValid)
            {
                dbo.Products.Add(product);
                dbo.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }
        [Authorize]
        public ActionResult Edit(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = dbo.Products.Find(ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,CategoryName,Price,Discount,SupplierName")] Product product)
        {
            if (ModelState.IsValid)
            {
                dbo.Entry(product).State = EntityState.Modified;
                dbo.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        [Authorize]
        public ActionResult Delete(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = dbo.Products.Find(ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string ProductId)
        {
            Product product = dbo.Products.Find(ProductId);
            dbo.Products.Remove(product);
            dbo.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}