using MIdProject.Models;
using MIdProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using MIdProject.Models.ViewModels;
namespace MIdProject.Controllers
{
    public class AdminController : Controller
    {
        ProjectEntities pe = new ProjectEntities();
        // GET: Admin
        public ActionResult CourseDetails()
        {
            List<Cours> courselist = pe.Courses.ToList();

            CourseViewModel cv = new CourseViewModel();

            List<CourseViewModel> cvl = courselist.Select(x => new CourseViewModel
            {
                Id = x.C_Id,
                Instructor_Name = x.User.Name,
                SubjectName = x.Subject.S_Name,
                C_Name = x.C_Name,
                C_Description = x.C_Description,
                Status = x.Status,
                Price = x.Price
            }).ToList();

            return View(cvl);
        }

        [HttpGet]
        public ActionResult CourseEdit(int id)
        {
            //var p = from item in inv.Products
            //        where item.ProductId == id
            //        select item;
            //Product product = p.FirstOrDefault();
            Subject st = new Subject();
            Cours p = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            p.C_Id = id;
            User[] Instructors = pe.Users.Where(x => x.Type == "Instructor").ToArray();
            Subject[] subjects = pe.Subjects.ToArray();
            Session["Instructors"] = Instructors;
            Session["Subjects"] = subjects;
            return View(p);
        }
        [HttpPost]
        public ActionResult CourseEdit(Cours c, int id)
        {
            Cours courseToUpdate = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            courseToUpdate.C_Id = id;
            courseToUpdate.C_Name = c.C_Name;
            courseToUpdate.Price = c.Price;
            courseToUpdate.Instructor_Id = c.Instructor_Id;
            courseToUpdate.Subject_Id = c.Subject_Id;
            //courseToUpdate.C_ = c.Subject_Id;

            pe.SaveChanges();
            return RedirectToAction("CourseDetails");
        }

        [HttpGet]
        public ActionResult CreateCourse()
        {
            Cours p = new Cours();
            User[] Instructors = pe.Users.Where(x => x.Type == "Instructor").ToArray();
            Subject[] subjects = pe.Subjects.ToArray();
            Session["Instructors"] = Instructors;
            Session["Subjects"] = subjects;

            return View(p);
        }

        [HttpPost]
        public ActionResult CreateCourse(Cours p)
        {
            if (ModelState.IsValid)
            {
                Notification n = new Notification();
                n.Notify_Name = p.C_Name + " Has been Added to Your Course List.Please Go to Show Courses For More Detail";
                n.Notify_No = "Enroll Now!!";
                n.AddedOn = DateTime.Now;
                n.Status = "Unread";
                n.Type = "All";
                pe.Notifications.Add(n);
                pe.SaveChanges();
                pe.Courses.Add(p);
                pe.SaveChanges();
                return RedirectToAction("CourseDetails");
            }
            else
            {
                return View(p);
            }                     
        }

        [HttpGet]
        public ActionResult CourseDelete(int id)
        {
            Cours p = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            return View(p);
        }

        [HttpPost, ActionName("CourseDelete")]
        public ActionResult ConfirmDelete(int id)
        {
            Cours p = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            pe.Courses.Remove(p);
            pe.SaveChanges();

            return RedirectToAction("CourseDetails");
        }

        [HttpGet]
        public ActionResult ManageStudent()
        {

            List<User> u = pe.Users.Where(x => x.Type == "Student" && x.Status == "Active").ToList();
            return View(u);
        }

        [HttpGet]
        public ActionResult ManageInstructor()
        {

            List<User> u = pe.Users.Where(x => x.Type == "Instructor" && x.Status == "Active").ToList();
            return View(u);
        }

        [HttpGet]
        public ActionResult ManageAdmin()
        {

            List<User> u = pe.Users.Where(x => x.Type == "Admin" && x.Status == "Active").ToList();
            return View(u);
        }

        [HttpGet]

        public ActionResult EditAdmin(int id)
        {
            User a = pe.Users.Where(x => x.Id == id).FirstOrDefault();

            return View(a);
        }

        [HttpPost]
        public ActionResult EditAdmin(User p, int id)
        {
            User userToUpdate = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            userToUpdate.Id = id;
            userToUpdate.Name = p.Name;
            userToUpdate.Email = p.Email;
            userToUpdate.Type = p.Type;
            userToUpdate.Status = p.Status;
            userToUpdate.Password = p.Password;
            pe.SaveChanges();
            if (p.Type == "Admin")
            {
                return RedirectToAction("ManageAdmin");
            }
            else if (p.Type == "Instructor")
            {
                return RedirectToAction("ManageInstructor");
            }
            else
            {
                return RedirectToAction("ManageStudent");
            }
        }

        [HttpGet]
        public ActionResult CreateAdmin()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(User p)
        {
            p.Type = "Admin";
            pe.Users.Add(p);
            pe.SaveChanges();
            return RedirectToAction("ManageAdmin");

        }
        [HttpGet]
        public ActionResult CreateStudent()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateStudent(User p)
        {
            p.Type = "Student";
            pe.Users.Add(p);
            pe.SaveChanges();
            return RedirectToAction("ManageStudent");

        }
        [HttpGet]
        public ActionResult CreateInstructor()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateInstructor(User p)
        {
            p.Type = "Instructor";
            pe.Users.Add(p);
            pe.SaveChanges();
            return RedirectToAction("ManageInstructor");

        }
        [HttpGet]
        public ActionResult AdminDelete(int id)
        {
            User p = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            return View(p);
        }

        [HttpPost, ActionName("AdminDelete")]
        public ActionResult ConfirmAdminDelete(int id)
        {
            User p = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            pe.Users.Remove(p);
            pe.SaveChanges();

            return RedirectToAction("ManageAdmin");
        }



        [HttpGet]
        public ActionResult Financials()
        {

            List<Financial> u = pe.Financials.ToList();
            return View(u);
        }

        public ActionResult Index()
        {
            GetData();
            EnrollData();
            FinancialData();
            return View();
        }

        public ActionResult GetData()
        {
            int instructor = pe.Users.Where(x => x.Type == "Instructor").Count();
            int student = pe.Users.Where(x => x.Type == "Student").Count();
            int admin = pe.Users.Where(x => x.Type == "Admin").Count();

            Session["instructor"] = instructor;
            Session["student"] = student;

            Ratio obj = new Ratio();
            obj.Instructor = instructor;
            obj.Student = student;
            obj.Admin = admin;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EnrollData()
        {
            /* int one = pe.Enrolls.Where(x => x.Course_Id == 1).Count();
             int two = pe.Enrolls.Where(x => x.Course_Id == 2).Count();
             int three = pe.Enrolls.Where(x => x.Course_Id == 3).Count();
             int four = pe.Enrolls.Where(x => x.Course_Id == 4).Count();
             int five = pe.Enrolls.Where(x => x.Course_Id == 5).Count();

             Enroll obj = new Enroll();
             obj.One = one;
             obj.Two = two;
             obj.Three = three;
             obj.Four = four;
             obj.Five = five;*/
            int c = pe.Courses.Count();
            EnrollViewModel obj = new EnrollViewModel();
            var cou = pe.Courses.ToList();
            EnrollViewModel en = new EnrollViewModel();
            List<int> count = new List<int>();
            List<string> str = new List<string>();
            IDictionary<string, int> dict = new Dictionary<string, int>();

            foreach (var v in cou)
            {

                int n = pe.Enrolls.Where(x => x.Course_Id == v.C_Id).Count();
                var m = pe.Courses.Where(x => x.C_Id == v.C_Id).FirstOrDefault();
                dict[m.C_Name] = n;

            }
            var items = from pair in dict
                        orderby pair.Value descending
                        select pair;
            foreach (KeyValuePair<string, int> pair in items)
            {
                count.Add(pair.Value);
                str.Add(pair.Key);
            }
            en.one = count[0];
            en.two = count[1];
            en.three = count[2];
            en.four = count[3];
            en.five = count[4];
            en.Name1 = str[0];
            en.Name2 = str[1];
            en.Name3 = str[2];
            en.Name4 = str[3];
            en.Name5 = str[4];


            return Json(en, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinancialData()
        {
            double prof = pe.Financials.Sum(x => x.Profit);
            double amount = pe.Financials.Sum(x => x.Amount);

            Session["amount"] = amount;

            FinanceRatio obj = new FinanceRatio();
            obj.Prof = prof;
            obj.Amount = amount;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageInactiveStudents()
        {
            List<User> a = pe.Users.Where(x => x.Status == "Inactive").ToList();

            return View(a);
        }

        [HttpGet]
        public ActionResult UnBan(int id)
        {
            User userToUpdate = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            // userToUpdate.Id = id;
            // userToUpdate.Name = p.Name;
            // userToUpdate.Email = p.Email;
            // userToUpdate.Type = "Active";
            userToUpdate.Status = "Active";
            // userToUpdate.Password = p.Password;
            pe.SaveChanges();
            return RedirectToAction("ManageInactiveStudents");

        }

        [HttpGet]
        public ActionResult Ban(int id)
        {
            User userToUpdate = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            // userToUpdate.Id = id;
            // userToUpdate.Name = p.Name;
            // userToUpdate.Email = p.Email;
            // userToUpdate.Type = "Active";
            userToUpdate.Status = "Inactive";
            // userToUpdate.Password = p.Password;
            pe.SaveChanges();
            return RedirectToAction("ManageInactiveStudents");

        }

       
    }
}