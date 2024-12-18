﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WineStore.Models;

namespace WineStore.Controllers
{
    public class ProductsController : Controller
    {
        private WineStoreDbContext db = new WineStoreDbContext();

        // GET: Products
        public ActionResult Index(string productName, string productPrice, string sortCriterion)
        {
            var products = db.Products.Select(p => p);
            ViewBag.sortByName = String.IsNullOrEmpty(sortCriterion) ? "NAME_ASC" : "NAME_DESC";
            ViewBag.sortByPrice = sortCriterion == "PRICE_ASC" ? "PRICE_ASC" : "PRICE_DESC";
            switch (sortCriterion)
            {
                case "NAME_ASC":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "NAME_DESC":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "PRICE_ASC":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "PRICE_DESC":
                    products = products.OrderBy(p => p.Price);
                    break;
            }
            if (!string.IsNullOrEmpty(productName))
            {
                products = products.Where(p => p.ProductName.Contains(productName));
            }
            if (!string.IsNullOrEmpty(productPrice))
            {
                decimal price = 0;
                if (decimal.TryParse(productPrice, out price))
                {
                    products = products.Where(p => p.Price >= price);
                }
            }
            return View(products.ToList());
        }

        // GET: Products/Details/5
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Description,PurchasePrice,Price,Quantity,Vintage,CatalogyID,Image,Region")] Product product)
        {
            if (ModelState.IsValid)
            {
                var productImageFile = Request.Files["productImageFile"];
                if (productImageFile != null && productImageFile.ContentLength > 0)
                {
                    String productImageFileName = System.IO.Path.GetFileName(productImageFile.FileName);
                    String serverPathForSavingProductImageFile = Server.MapPath("~/Images/" + productImageFileName);
                    productImageFile.SaveAs(serverPathForSavingProductImageFile);
                    product.Image = productImageFileName;
                }
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
            return View(product);
        }

        // GET: Products/Edit/5
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
            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Description,PurchasePrice,Price,Quantity,Vintage,CatalogyID,Image,Region")] Product product)
        {
            if (ModelState.IsValid)
            {
                var persistentProduct = db.Products.AsNoTracking().SingleOrDefault(p => p.ProductID == product.ProductID);
                var productImageFile = Request.Files["productImageFile"];
                if (productImageFile != null && productImageFile.ContentLength > 0)
                {
                    String productImageFileName = System.IO.Path.GetFileName(productImageFile.FileName);
                    String serverPathForSavingProductImageFile = Server.MapPath("~/Images/" + productImageFileName);
                    productImageFile.SaveAs(serverPathForSavingProductImageFile);
                    product.Image = productImageFileName;
                }
                else
                {
                    product.Image = persistentProduct.Image;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
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
