using MIdProject.Models;
using MIdProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIdProject.Controllers
{
    public class HomeController : Controller
    {
        ProjectEntities pe = new ProjectEntities();
        // GET: Auth
        public ActionResult Index()
        {
            if (Session["id"] != null)
            {
                var n = pe.Notifications.Where(x => x.Type == "All" && x.Status == "Unread").FirstOrDefault();
                if (n != null)
                {
                    Session["noti"] = "Success";
                }
                var k = pe.Notifications.Where(x => x.Type == "Notice" && x.Status == "Unread").FirstOrDefault();
                if (k != null)
                {
                    var c = pe.Courses.Where(x => x.C_Name == k.Notify_No).FirstOrDefault();
                    string email = Session["email"].ToString();
                    var g = pe.Enrolls.Where(x => x.Student_Email == email && x.Course_Id == c.C_Id).FirstOrDefault();
                    if (g != null)
                    {
                        Session["notify"] = "Success";
                    }
                }

            }

            if (Session["type"] == null)
            {
                return View(pe.Courses.ToList());
            }
            if (Session["type"].ToString() == "Student")
            {
                string email = Session["email"].ToString();
                List<Cours> cr = new List<Cours>();
                List<Enroll> en = pe.Enrolls.Where(x => x.Student_Email == email).ToList();
                foreach (var m in en)
                {
                    var p = pe.Courses.Where(x => x.C_Id == m.Course_Id).FirstOrDefault();
                    cr.Add(p);
                }

                return View(cr);
            }
            else
            {
                return View(pe.Courses.ToList());
            }
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel u)
        {
            int noti = pe.Notifications.Where(x => x.Status == "Unread").Count();
            Session["cou"] = noti;

            var logins = pe.Users.Where(x => x.Email == u.Email && x.Password == u.Password).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (logins == null)
                {
                    TempData["Messege"] = "Username or Password is incorrect";


                    return View("Login", u);
                }
                else if (logins.Status == "Inactive")
                {
                    TempData["Messege"] = "Your Account is Inactive.Please Contact with the Admin";


                    return View("Login", u);
                }
                else
                {
                    TempData["Messege"] = null;
                    if (logins.Type == "Instructor")
                    {
                        Session["uname"] = logins.Name;
                        Session["type"] = logins.Type;
                        Session["id"] = logins.Id;
                        Session["email"] = logins.Email;
                        return RedirectToAction("Index");
                    }
                    else if (logins.Type == "Student")
                    {
                        Session["uname"] = logins.Name;
                        Session["type"] = logins.Type;
                        Session["id"] = logins.Id;
                        Session["email"] = logins.Email;
                        Session["Counter"] = 0;
                        return RedirectToAction("Index");
                    }
                    else if (logins.Type == "Admin")
                    {
                        Session["uname"] = logins.Name;
                        Session["type"] = logins.Type;
                        Session["id"] = logins.Id;
                        Session["email"] = logins.Email;
                        return RedirectToAction("Index", "Admin");
                    }

                }

            }
            return View(u);
        }
        [HttpGet]
        public ActionResult Signup()
        {

            return View();

        }
        [HttpPost]
        public ActionResult Signup(RegistrationViewModel u)
        {

            User check = pe.Users.Where(x => x.Email == u.Email).FirstOrDefault();
            User user = new User();

            if (check != null)
            {
                TempData["Exist"] = "Email Already Exists";


                return View("Signup", u);
            }
            if (ModelState.IsValid)
            {
                user.Name = u.Name;
                user.Email = u.Email.ToLower();
                user.Password = u.Password;
                user.Type = "Student";
                user.Status = "Active";
                pe.Users.Add(user);
                pe.SaveChanges();
                TempData["Exists"] = null;
                return RedirectToAction("Login");
            }
            else
            {
                return View(u);
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session["uname"] = null;
            Session["email"] = null;
            Session["type"] = null;
            Session["id"] = null;
            Session["cou"] = 0;
            return RedirectToAction("Login");
        }
        /*public ActionResult GetNotifications()
        {
            var n = pe.Notifications.Where(x => x.Type == "All" && x.Status == "Unread").FirstOrDefault();
            return Json(n, JsonRequestBehavior.AllowGet);
        }*/
        public ActionResult CheckNotification()
        {
            if (Session["id"] != null)
            {
                var n = pe.Notifications.ToList();
                Session["noti"] = null;
                Session["notify"] = null;
                Session["cou"] = 0;
                foreach (var m in n)
                {
                    m.Status = "Read";
                    pe.SaveChanges();
                }
                if (Session["type"].ToString() == "Student")
                {
                    List<Notification> nt = pe.Notifications.Where(x => x.Type == "All").ToList();
                    var k = pe.Notifications.Where(x => x.Type == "Notice").ToList();
                    string email = Session["email"].ToString();
                    foreach (var m in k)
                    {
                        var c = pe.Courses.Where(x => x.C_Name == m.Notify_No).FirstOrDefault();
                        var g = pe.Enrolls.Where(x => x.Student_Email == email && x.Course_Id == c.C_Id).FirstOrDefault();
                        if (g != null)
                        {
                            nt.Add(m);
                        }

                    }
                    var l = nt.OrderByDescending(x => x.AddedOn.Date).ToList();


                    return View(l);
                }

                else
                {
                    var lt = n.OrderByDescending(x => x.AddedOn.Date).ToList();
                    return View(lt);
                }


            }
            else
            {
                return View();
            }
        }
    }
}