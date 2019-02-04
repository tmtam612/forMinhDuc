using Client.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
namespace Client.Controllers
{
    public class EmployeesController : Controller
    {
        private pro_sem3Entities db = new pro_sem3Entities();

        // GET: Employees
        //public ActionResult Index()
        //{
        //    var employees = db.Employees.Include(e => e.Position);
        //    return View(employees.ToList());
        //}

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.FirstOrDefault(x=>x.username==id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "positionName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,username,fullname,password,phone,address,email,PositionID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.password = MD5Hash(employee.password);
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("ManageEmployee");
            }

            ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "positionName", employee.PositionID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.FirstOrDefault(x=>x.username==id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "positionName", employee.PositionID);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,username,fullname,password,phone,address,email,PositionID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { @id = Session["userName"].ToString() }); 
            }
            ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "positionName", employee.PositionID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        public ActionResult ManageEmployee(string searchString, int? page)
        {
            if(Session["userName"]==null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Session["userName"].ToString();
            var model1 = db.Employees.FirstOrDefault(x => x.username == id);
            ViewBag.position = model1.PositionID;
            var model2 = from d in db.Employees select d;
            if(model1.PositionID==1)
            {
                model2 = from d in db.Employees
                             where d.PositionID!=1
                             select d;
            }
            if(model1.PositionID==2)
            {
                model2 = from d in db.Employees where d.PositionID != 1 && d.PositionID != 2 select d;
            }
            if(model1.PositionID==3)
            {
                model2 = from d in db.Employees where d.PositionID != 1 && d.PositionID != 2 && d.PositionID!=3 select d;
            }
            if(!string.IsNullOrEmpty(searchString))
            {
                page = 1;
                model2 = model2.Where(x => x.username.Contains(searchString)||x.fullname.Contains(searchString));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model2.OrderBy(i => i.EmployeeID).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Logout()
        {
            Session["userName"] = null;
            return RedirectToAction("Index","Home");
        }
        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("ManageEmployee");
        }
        public ActionResult checkBill(string searchString,int? page)
        {
            var model = from d in db.bills select d;
            if(!string.IsNullOrEmpty(searchString))
            {
                page = 1;
                var id = Convert.ToInt32(searchString);
                model = model.Where(x => x.billID == id);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(i => i.date).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult checkBilled(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bill bill = db.bills.FirstOrDefault(x => x.billID == id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);

        }
        [HttpPost,ActionName("checkBilled")]
        public ActionResult CheckBilled(int id)
        {
            bill bill = db.bills.FirstOrDefault(x => x.billID == id);
            bill.status = true;
            db.SaveChanges();
            return RedirectToAction("checkBill");
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
