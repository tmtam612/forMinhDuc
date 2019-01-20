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
using PagedList;
namespace Client.Controllers
{
    public class billsController : Controller
    {
        private pro_sem3Entities1 db = new pro_sem3Entities1();
        private void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";

            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";

            }
        }
        // GET: bills
        public ActionResult Index(string searchString,string typeSearch,string statusString,int? page)
        {
            if (Session["userName"] != null)
            {
                List<SelectListItem> listItems1 = new List<SelectListItem>();
                listItems1.Add(new SelectListItem
                {
                    Text = "Date",
                    Value = "Date"
                });
                listItems1.Add(new SelectListItem
                {
                    Text = "BillID",
                    Value = "BillID",
                    Selected = true
                });
                List<SelectListItem> listItems2 = new List<SelectListItem>();
                listItems2.Add(new SelectListItem
                {
                    Text = "Done",
                    Value = "Done"
                });
                listItems2.Add(new SelectListItem
                {
                    Text = "Doing",
                    Value = "Doing",
                    Selected = true
                });
                ViewBag.listItems1 = listItems1;
                ViewBag.listItems2 = listItems2;
                var userName = Session["userName"].ToString();
                var bills = from d in db.bills where d.username == userName select d;
                
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (!string.IsNullOrEmpty(searchString))
                {
                    if (!String.IsNullOrEmpty(typeSearch))
                    {
                        page = 1;
                        if (typeSearch == "Date")
                        {
                            try
                            {
                                DateTime dateTime = DateTime.ParseExact(searchString, "MM/dd/yyyy", null);
                                bills = bills.Where(x => x.date == dateTime);
                            }
                            catch
                            {
                                SetAlert("search string has wrong format", "warning");
                                return View(bills.OrderBy(i => i.billID).ToPagedList(pageNumber, pageSize));
                            }
                        }
                        else if (typeSearch == "BillID")
                        {
                            var id = Convert.ToInt32(searchString);
                            bills = bills.Where(x => x.billID == id);
                        }
                    }
                }
                if(!string.IsNullOrEmpty(statusString))
                {
                    if (statusString == "Done")
                    {
                        bills = bills.Where(x => x.status == true);
                    }
                    else if (statusString == "Doing")
                    {
                        bills = bills.Where(x => x.status == false);
                    }
                }
                return View(bills.OrderBy(i => i.billID).ToPagedList(pageNumber, pageSize));
            }
            else return RedirectToAction("Index","Home");
           
        }

        // GET: bills/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bill bill = db.bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // GET: bills/Create
        public ActionResult Create()
        {
            if (Session["userName"] != null)
            {
                ViewBag.locationID = new SelectList(db.locations, "locationID", "name");
                return View();
            }
            else return RedirectToAction("Index", "Home");
        }

        // POST: bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "proID,username,description,name,receivername,sentname,sentaddress,receiveraddress,locationID,quantity")] bill bill)
        {
            if (Session["userName"] != null)
            {
                if (ModelState.IsValid)
                {
                    bill.username = Session["userName"].ToString();
                    bill.billID = db.bills.Count() + 1;
                    TempData["bill"] = bill;
                    return RedirectToAction("CreateID", "products");
                }
                ViewBag.locationID = new SelectList(db.locations, "locationID", "name", bill.locationID);
                return View(bill);
            }
            else return RedirectToAction("Index", "Home");
        }
        #region
        public decimal priceForWeight(decimal weight)
        {
            if(weight<200)
            {
                return 25000;
            }
            else if(weight>=200&&weight<500)
            {
                return 35000;
            }
            else if(weight>=500&&weight<1000)
            {
                return 45000;
            }
            else if(weight>=1000&&weight<1500)
            {
                return 60000;
            }
            else
            {
                var range = (weight - 1500) / 500;
                if(range>=0&&range<1)
                {
                    return 70000;
                }
                else
                {
                    var price = Math.Round(range) * 10000 + 60000;
                    return price;
                }
            }
        }
        public decimal priceForHeight(decimal Height)
        {
            if (Height < 2)
            {
                return 5000;
            }
            else if (Height >= 2 && Height < 10)
            {
                return 10000;
            }
            else if (Height >= 10 && Height < 20)
            {
                return 15000;
            }
            else if (Height >= 20 && Height < 50)
            {
                return 25000;
            }
            else
            {
                var range = (Height - 50) / 20;
                if (range >= 0 && range < 1)
                {
                    return 45000;
                }
                else
                {
                    var price = Math.Round(range) * 20000 + 25000;
                    return price;
                }
            }
        }
        public decimal priceForWidth(decimal Width)
        {
            if (Width < 5)
            {
                return 5000;
            }
            else if (Width >= 5 && Width < 10)
            {
                return 10000;
            }
            else if (Width >= 10 && Width < 20)
            {
                return 20000;
            }
            else if (Width >= 20 && Width < 50)
            {
                return 45000;
            }
            else
            {
                var range = (Width - 50) / 10;
                if (range >= 0 && range < 1)
                {
                    return 55000;
                }
                else
                {
                    var price = Math.Round(range) * 10000 + 45000;
                    return price;
                }
            }
        }
        #endregion
        public ActionResult Createbill()
        {
            if (Session["userName"] != null)
            {
                var model = TempData["mydata"] as List<product>;
                var modelBill = TempData["bill"] as bill;
                if (model != null && modelBill != null)
                {
                    var locationTax = db.locations.FirstOrDefault(x => x.locationID == modelBill.locationID).price;
                    decimal total = 0;
                    foreach (var item in model)
                    {
                        var priceForWeight = this.priceForWeight(item.weight);
                        var priceForHeight = this.priceForHeight(item.height);
                        var priceForWidth = this.priceForWidth(item.width);
                        total = total + priceForHeight + priceForWeight + priceForWidth;
                    }
                    ViewBag.locationName = db.locations.FirstOrDefault(x => x.locationID == modelBill.locationID).name;
                    modelBill.total = total + locationTax + total * 10 / 100;
                    modelBill.date = DateTime.Now;
                    TempData["mydata"] = model;
                    TempData["bill"] = modelBill;
                    return View(modelBill);
                }
                else return RedirectToAction("Create", "bills");
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost,ActionName("Createbill")]
        public ActionResult AddBill()
        {
            if (Session["userName"] != null)
            {
                var model = TempData["mydata"] as List<product>;
                var modelBill = TempData["bill"] as bill;
                if (model != null && modelBill != null)
                {
                    modelBill.status = false;
                    var location = db.locations.FirstOrDefault(x => x.locationID == modelBill.locationID);
                    modelBill.location = location;
                    db.bills.Add(modelBill);
                    foreach (var item in model)
                    {
                        item.bill = modelBill;
                        item.description = "abc";
                        db.products.Add(item);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else return RedirectToAction("Create", "bills");
            }
            else return RedirectToAction("Index", "Home");
        }
        // GET: bills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bill bill = db.bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            ViewBag.locationID = new SelectList(db.locations, "locationID", "name", bill.locationID);
            return View(bill);
        }

        // POST: bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "proID,username,weight,lenght,width,description,name,receivername,sentname,sentaddress,receiveraddress,total,locationID,quantity")] bill bill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.locationID = new SelectList(db.locations, "locationID", "name", bill.locationID);
            return View(bill);
        }

        // GET: bills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bill bill = db.bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // POST: bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bill bill = db.bills.Find(id);
            db.bills.Remove(bill);
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
