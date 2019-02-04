using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Client.Models;

namespace Client.Controllers
{
    public class productsController : Controller
    {
        private pro_sem3Entities db = new pro_sem3Entities();

        // GET: products
        public ActionResult Index()
        {
            var products = db.products.Include(p => p.bill);
            return View(products.ToList());
        }
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.products.Where(x => x.billID == id).ToList();
            if(product==null)
            {
                return HttpNotFound();
            }
            return View(product);
            
        }
        // GET: products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: products/Create
        public ActionResult CreateID()
        {
            if (Session["userName"] != null)
            {
                var id = TempData["bill"] as bill;
                if (id != null)
                {
                    List<product> l = new List<product>(id.quantity);
                    for (var i = 0; i < id.quantity; i++)
                    {
                        product product = new product();
                        product.billID = id.billID;
                        l.Add(product);
                    }
                    TempData["bill"] = id;
                    ViewBag.proID = new SelectList(db.bills, "proID", "username");
                    return View(l);
                }
                else return RedirectToAction("Create", "bills");
            }
            else return RedirectToAction("Index", "Home");
        }

        // POST: products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("CreateID")]
        [ValidateAntiForgeryToken]
        public ActionResult Create( List<product> product)
        {
            if (Session["userName"] != null)
            {
                var modelBill = TempData["bill"] as bill;
                decimal height = 0;
                decimal length = 0;
                decimal width = 0;
                decimal weight = 0;
                foreach (var item in product)
                {
                    if (item.height > height) height = item.height;
                    weight += item.weight;
                    width += item.width;
                    length += item.lenght;
                }
                modelBill.lenght = length;
                modelBill.width = width;
                modelBill.weight = weight;
                modelBill.height = height;
                TempData["bill"] = modelBill;
                TempData["mydata"] = product;
                return RedirectToAction("Createbill", "bills");
            }
            else return RedirectToAction("Index", "Home");

            //ViewBag.proID = new SelectList(db.bills, "proID", "username", product.proID);
            //return View(product);
        }
        public ActionResult EditID()
        {
            if (Session["userName"] != null)
            {
                var id = TempData["bill"] as bill;
                if (id != null)
                {
                    List<product> l = new List<product>(id.quantity);
                    for (var i = 0; i < id.quantity; i++)
                    {
                        product product = new product();
                        product.billID = id.billID;
                        l.Add(product);
                    }
                    TempData["bill"] = id;
                    ViewBag.proID = new SelectList(db.bills, "proID", "username");
                    return View(l);
                }
                else return RedirectToAction("Create", "bills");
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost, ActionName("EditID")]
        [ValidateAntiForgeryToken]
        public ActionResult EditIDs(List<product> product)
        {
            if (Session["userName"] != null)
            {
                var modelBill = TempData["bill"] as bill;
                decimal height = 0;
                decimal length = 0;
                decimal width = 0;
                decimal weight = 0;
                foreach (var item in product)
                {
                    if (item.height > height) height = item.height;
                    weight += item.weight;
                    width += item.width;
                    length += item.lenght;
                }
                modelBill.lenght = length;
                modelBill.width = width;
                modelBill.weight = weight;
                modelBill.height = height;
                TempData["bill"] = modelBill;
                TempData["mydata"] = product;
                return RedirectToAction("EditBill", "bills");
            }
            else return RedirectToAction("Index", "Home");

            //ViewBag.proID = new SelectList(db.bills, "proID", "username", product.proID);
            //return View(product);
        }
        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.proID = new SelectList(db.bills, "proID", "username", product.proID);
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "proID,weight,lenght,height,width,description,name")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.proID = new SelectList(db.bills, "proID", "username", product.proID);
            return View(product);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
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
