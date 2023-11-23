using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;

namespace CozaStore.Areas.Dashboard.Controllers
{
    public class BannersController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Banners
        public ActionResult Index()
        {
            var banner = db.Banners.Include(b => b.Product);
            return View(banner.ToList());
        }

        // GET: Dashboard/Banners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // GET: Dashboard/Banners/Create
        public ActionResult Create()
        {
            ViewBag.Productid = db.Products.ToList();
            return View();
        }

        // POST: Dashboard/Banners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Bannerid,Productid,ContentHigh,Contentlow")] Banner banner)
        {
            if (ModelState.IsValid)
            {
                db.Banners.Add(banner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Productid = new SelectList(db.Products, "Productid", "Name", banner.Productid);
            return View(banner);
        }

        // GET: Dashboard/Banners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            ViewBag.Productid = new SelectList(db.Products, "Productid", "Name", banner.Productid);
            return View(banner);
        }

        // POST: Dashboard/Banners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Bannerid,Productid,ContentHigh,Contentlow")] Banner banner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Productid = new SelectList(db.Products, "Productid", "Name", banner.Productid);
            return View(banner);
        }

     
        public ActionResult Delete(int id)
        {
            Banner banner = db.Banners.Find(id);
            db.Banners.Remove(banner);
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
