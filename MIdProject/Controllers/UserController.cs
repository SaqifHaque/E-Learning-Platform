using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIdProject.Models;
using MIdProject.Models.ViewModels;

namespace MIdProject.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        ProjectEntities pe = new ProjectEntities();
        public List<CartViewModel> listOfCart = new List<CartViewModel>();

        public ActionResult ShowCourses()
        {
            string email = Session["email"].ToString();
            List<Cours> cr = pe.Courses.ToList();
            List<Enroll> en = pe.Enrolls.Where(x => x.Student_Email == email).ToList();
            if (en != null)
            {            
                foreach (var m in en)
                {
                    var p = pe.Courses.Where(x => x.C_Id == m.Course_Id).FirstOrDefault();
                    cr.RemoveAll(x => x.C_Id == p.C_Id);
                }
                //List<Cours> lr = cr.Distinct().ToList();
               

                return View(cr);
            }
            else
            {
                return View(pe.Courses.ToList());
            }
            
        }
        public ActionResult CourseContent(int id)
        {
            List<Content> con = pe.Contents.Where(x => x.Course_Id == id).ToList();
            var co = pe.Courses.Where(y => y.C_Id == id).FirstOrDefault();
            var us = pe.Users.Where(z => z.Id == co.Instructor_Id).FirstOrDefault();

            List<ContentViewModel> cvl = con.Select(x => new ContentViewModel
            {
                Course_Id = x.Cours.C_Id,
                Course_Name = x.Cours.C_Name,
                Instructor_Name = x.Cours.User.Name,
                File_Name = x.File_Name,
                File_Path = x.File_Path
            }).ToList();
            Session["subject_id"] = id;

            return View(cvl);
        }
        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~/Uploaded/" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public ActionResult CourseDetails(int id)
        {
            var p = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            var i = pe.Users.Where(x => x.Id == p.Instructor_Id).FirstOrDefault();
            var s = pe.Subjects.Where(x => x.S_Id == p.Subject_Id).FirstOrDefault();
            /*CourseViewModel cv = new CourseViewModel();
            cv.C_Name = p.C_Name;
            cv.Instructor_Name = i.Name;
            cv.Price = p.Price;
            cv.SubjectName = s.S_Name;
            cv.C_Description = p.C_Description;

            return View(cv);*/

            dynamic dy = new ExpandoObject();
            dy.coursedetails = p;
            dy.userdetails = i;
            dy.subjectdetails = s;

            return View(dy);

        }


        public ActionResult AddToCart(int itemId)
        {
            var obj = pe.Courses.Where(x => x.C_Id == itemId).FirstOrDefault();
            var c = pe.Carts.Where(x => x.Item_Id == obj.C_Id).FirstOrDefault();
            if (c == null)
            {
                Cart cr = new Cart();
                cr.Item_Id = obj.C_Id;
                cr.Student_Id = (int)Session["id"];
                pe.Carts.Add(cr);
                pe.SaveChanges();
                return RedirectToAction("ShowCart");
            }
            else
            {
                TempData["Cart"] = "It's Already added in the Cart";
                return RedirectToAction("ShowCart");
            }

        }

        public ActionResult ShowCart()
        {
            CartViewModel cv = new CartViewModel();
            int id = (int)Session["id"];
            var c = pe.Carts.Where(x => x.Student_Id == id).ToList();
            Session["Counter"] = c.Count;
            List<CartViewModel> cvl = c.Select(x => new CartViewModel
            {
                C_Name = x.Cours.C_Name,
                Price = x.Cours.Price,
                Cart_Id = x.Cart_Id
            }).ToList();
            return View(cvl);

        }
        [HttpGet]
        public ActionResult CheckOut()
        {
            Payment pm = new Payment();
            return View(pm);
        }
        [HttpPost]
        public ActionResult CheckOut(Payment pm)
        {
            if (ModelState.IsValid)
            {
                DateTime cdr = DateTime.Now;
                string completion = cdr.AddDays(30).ToString();
                int id = (int)Session["id"];
                var cr = pe.Carts.Where(x => x.Student_Id == id).ToList();
                var u = pe.Users.Where(x => x.Id == id).FirstOrDefault();
                Enroll en = new Enroll();
                Financial fn = new Financial();
                Invoice inv = new Invoice();
                foreach (var m in cr)
                {
                    var c = pe.Carts.Where(x => x.Student_Id == id).FirstOrDefault();
                    var co = pe.Courses.Where(x => x.C_Id == m.Item_Id).FirstOrDefault();
                    en.Course_Id = co.C_Id;
                    en.Instructor_Id = co.Instructor_Id;
                    en.Student_Email = u.Email;
                    en.Date_Of_Enrollment = cdr.ToString();
                    en.Date_Of_Completion = completion;
                    en.Status = "Incomplete";
                    pe.Enrolls.Add(en);
                    pe.SaveChanges();
                    inv.Items += co.C_Name + ",";
                    inv.Price += (float)co.Price;
                    pe.Carts.Remove(m);
                    pe.SaveChanges();
                }

                inv.Student_Id = u.Id;
                inv.Transaction_Id = pm.Transaction;
                pe.Invoices.Add(inv);
                pe.SaveChanges();
                List<Invoice> ls = pe.Invoices.ToList();
                inv = ls.LastOrDefault();
                fn.Invoice_Id = inv.Invoice_Id;
                fn.Courses = inv.Items;
                fn.Paid_By = id;
                fn.Amount = (int)inv.Price;
                fn.Profit = inv.Price * 10 / 100;
                pe.Financials.Add(fn);
                pe.SaveChanges();
                Session["Counter"] = 0;

                return RedirectToAction("Invoice");
            }
            else
            {
                return View(pm);
            }
        }
        public ActionResult MyProfile(int id)
        {
            var details = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            return View(details);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            EditProfileViewModel ed = new EditProfileViewModel();

            User user = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            ed.Id = user.Id;
            ed.Name = user.Name;
            ed.Email = user.Email;
            ed.Type = user.Type;
            ed.Status = user.Status;
            ed.Password = user.Password;
            return View(ed);
        }

        [HttpPost]
        public ActionResult Edit(EditProfileViewModel user, int id)
        {
            User userToUpdate = pe.Users.Where(x => x.Id == id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                userToUpdate.Id = user.Id;
                userToUpdate.Name = user.Name;
                userToUpdate.Email = user.Email.ToLower();
                userToUpdate.Type = user.Type;
                userToUpdate.Status = user.Status;
                userToUpdate.Password = user.Password;
                pe.SaveChanges();
                Session["uname"] = user.Name;
                return RedirectToAction("MyProfile", new { id = userToUpdate.Id });
            }
            else
            {
                return View("Edit", user);
            }

        }

        public ActionResult Notice(int id)
        {
            List<Notice> n = pe.Notices.Where(x => x.Course_Id == id).ToList();

            return View(n);
        }
        public ActionResult ShowVideo(int id)
        {
            List<Video> n = pe.Videos.Where(x => x.Course_Id == id).ToList();

            return View(n);
        }
        public ActionResult RemoveCart()
        {
            int id = (int)Session["id"];
            List<Cart> n = pe.Carts.Where(x => x.Student_Id == id).ToList();
            foreach (var m in n)
            {
                pe.Carts.Remove(m);
                pe.SaveChanges();
            }

            return RedirectToAction("ShowCart", "User");
        }
        [HttpGet]
        public ActionResult Invoice()
        {
            int id = (int)Session["id"];
            var c = pe.Invoices.Where(x => x.Student_Id == id).ToList();
            return View(c);
        }


        [HttpGet]
        public ActionResult CheckMessages()
        {
            if (Session["subject_id"] != null)
            {
                int id = (int)Session["subject_id"];

                string mail = Session["email"].ToString();
                List<Email> emails = pe.Emails.Where(x => x.Couse_Id == id && x.receiver == mail).ToList();
                return View(emails);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult CheckSentMessages()
        {
            if (Session["subject_id"] != null)
            {
                int id = (int)Session["subject_id"];
                string mail = Session["email"].ToString();
                List<Email> emails = pe.Emails.Where(x => x.Couse_Id == id && x.sender == mail).ToList();
                return View(emails);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]

        public ActionResult SendMessage()
        {
            Email em = new Email();


            return View(em);
        }
        [HttpPost]

        public ActionResult SendMessage(Email em)
        {

            em.Couse_Id = (int)Session["subject_id"];
            var c = pe.Courses.Where(x => x.C_Id == em.Couse_Id).FirstOrDefault();
            var r = pe.Users.Where(x => x.Id == c.Instructor_Id).FirstOrDefault();
            em.sender = Session["email"].ToString();
            em.receiver = r.Email;

            pe.Emails.Add(em);
            pe.SaveChanges();
            return RedirectToAction("CheckSentMessages", "User");
        }


        [HttpGet]
        public ActionResult DeleteMessage(int id)
        {
            var email = pe.Emails.Where(x => x.id == id).FirstOrDefault();
            pe.Emails.Remove(email);
            pe.SaveChanges();
            return RedirectToAction("CheckMessages", "User");
        }



    }
}