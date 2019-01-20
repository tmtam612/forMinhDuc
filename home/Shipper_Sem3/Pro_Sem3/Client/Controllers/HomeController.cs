using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Client.Models;
namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private pro_sem3Entities1 db = new pro_sem3Entities1();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost,ActionName("Signup")]
        public ActionResult Signup([Bind(Include = "memID,username,fullname,pass,phone,address,email")] Member member)
        {
            if(ModelState.IsValid)
            {
                var model = db.Members.FirstOrDefault(x => x.username == member.username);
                if(model!=null)
                {
                    SetAlert("User Name have been exist", "warning");
                    return RedirectToAction("Signup");
                }
                else
                {
                    db.Members.Add(member);
                    db.SaveChanges();
                    SetAlert("Signun successfully", "success");
                }                
            }
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Session["userName"] = null;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Login(string userName, string passWord)
        {
            if (userName != "" && passWord != "")
            {
                var pass = MD5Hash(passWord);
                var result = db.Members.FirstOrDefault(x => x.username == userName);
                if (result != null)
                {
                    if (result.pass == pass.ToUpper())
                    {
                        Session["userName"] = userName;
                        SetAlert("Log in Successfully", "success");
                        return View();
                    }
                    else
                    {
                        SetAlert("Wrong password", "error");
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    SetAlert("The account does not exist", "error");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                SetAlert("Please type your username and password", "warning");
                return RedirectToAction("Index", "Home");
            }
        }
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
    }
}