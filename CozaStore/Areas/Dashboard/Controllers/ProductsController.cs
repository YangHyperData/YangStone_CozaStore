using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
using System.IO;
namespace CozaStore.Areas.Dashboard.Controllers
{
    public class ProductsController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Products
        public ActionResult Index()
        {
            var product = db.Products.Include(p => p.Category);
            return View(product.ToList());
        }

        // GET: Dashboard/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Dashboard/Products/Create
        public ActionResult Create()
        {
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name");
            return View();
        }

        // POST: Dashboard/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Product product, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            if(fFileUpload != null)
            {
                if (ModelState.IsValid)
                {
                    product.Name = f["ProductName"];
                    product.Describe = string.Format(f["Describe"].ToString());
                    product.Price = decimal.Parse(f["Price"].ToString());
                    product.Amount = int.Parse(f["Amount"].ToString());
                    product.Categoryid = int.Parse(f["Categoryid"].ToString());
                    var FileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Asset/images"), FileName);
                    product.Illsutration = FileName;
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    db.Products.Add(product);
                    db.SaveChanges();
                    @TempData["MessageAlert"] = "Create product " + product.Name;
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name", product.Categoryid);
            return View(product);
        }

        // GET: Dashboard/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name", product.Categoryid);
            return View(product);
        }

        // POST: Dashboard/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       [ValidateInput(false)]
        public ActionResult Edit(int id, HttpPostedFileBase fFileUpload, FormCollection f)
        {
            Product product = db.Products.SingleOrDefault(n => n.Productid == id);
            if (ModelState.IsValid)
            {
               if(fFileUpload != null)
                {
                    var FileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Asset/images"), FileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    product.Illsutration = FileName;
                }
                product.Name = f["Name"];
                product.Describe = f["Describe"];
                product.Price = decimal.Parse(f["Price"].ToString());
                product.Amount = int.Parse(f["Amount"].ToString());
                product.Categoryid = int.Parse(f["Categoryid"].ToString());
                @TempData["MessageAlert"] = "Edit product " + product.Name; 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categoryid = new SelectList(db.Categories, "Categoryid", "Name", product.Categoryid);
            return View(product);
        }
        // POST: Dashboard/Products/Delete/5

        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            var banner = from c in db.Banners
                         where c.Productid == id
                         select c;
            var OrderDetails = from c in db.DetailsOrders
                               where c.Productid == id
                               select c;
            db.Banners.RemoveRange(banner);
            db.DetailsOrders.RemoveRange(OrderDetails);
            @TempData["MessageAlert"] = "Delete product " + product.Name;
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
