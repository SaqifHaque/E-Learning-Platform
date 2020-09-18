using MIdProject.Models;
using MIdProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIdProject.Controllers
{
    public class InstructorController : Controller
    {
        // GET: Instructor

        ProjectEntities pe = new ProjectEntities();

        [HttpGet]
        public ActionResult ShowCourses(int id)
        {
            List<Cours> courses = new List<Cours>();
            courses = pe.Courses.Where(t => t.Instructor_Id == id).ToList();
            return View(courses);
        }

        [HttpGet]

        public ActionResult CourseDetails(int id)
        {

            var p = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
            var i = pe.Users.Where(x => x.Id == p.Instructor_Id).FirstOrDefault();
            var s = pe.Subjects.Where(x => x.S_Id == p.Subject_Id).FirstOrDefault();
            Content cn = new Content();
            List<Enroll> enrolls = new List<Enroll>();
            enrolls = pe.Enrolls.Where(a => a.Course_Id == id).ToList();

            dynamic dy = new ExpandoObject();
            dy.coursedetails = p;
            dy.userdetails = i;
            dy.subectdetails = s;
            dy.studentlist = enrolls;
            dy.content = cn;

            return View(dy);
        }

        [HttpGet]
        public ActionResult UploadContent(int id)
        {
            List<Content> con = pe.Contents.Where(x => x.Course_Id == id).ToList();
            var co = pe.Courses.Where(y => y.C_Id == id).FirstOrDefault();
            var us = pe.Users.Where(z => z.Id == co.Instructor_Id).FirstOrDefault();

            List<ContentViewModel> cvl = con.Select(x => new ContentViewModel
            {
                Course_Name = x.Cours.C_Name,
                Instructor_Name = x.Cours.User.Name,
                File_Name = x.File_Name,
                File_Path = x.File_Path
            }).ToList();

            return View(cvl);
        }

        [HttpPost]
        public ActionResult UploadContent(HttpPostedFileBase file, int id)
        {
            var course = pe.Courses.Where(z => z.C_Id == id).FirstOrDefault();

            string FileName = Path.GetFileName(file.FileName);
            string FilePath = Path.Combine(Server.MapPath("~/Uploaded"), FileName);

            Content content = new Content();
            content.Course_Id = course.C_Id;
            content.File_Name = FileName;
            content.File_Path = FilePath;
            file.SaveAs(FilePath);

            //Content con = new Content();
            //con.Course_Id = content.Course_Id;
            //con.Instructor_Id = content.Instructor_Id;

            pe.Contents.Add(content);
            pe.SaveChanges();
            return RedirectToAction("UploadContent");
        }

        [HttpGet]
        public ActionResult PostNotice(int id)
        {
            Notice no = new Notice();
            return View(no);
        }
        [HttpPost]
        public ActionResult PostNotice(Notice no, int id)
        {
            if (ModelState.IsValid)
            {
                Notification n = new Notification();
                var c = pe.Courses.Where(x => x.C_Id == id).FirstOrDefault();
                n.Notify_Name = no.Notice1;
                n.Notify_No = c.C_Name;
                n.AddedOn = DateTime.Now;
                n.Status = "Unread";
                n.Type = "Notice";
                pe.Notifications.Add(n);
                pe.SaveChanges();
                no.Course_Id = id;
                pe.Notices.Add(no);
                pe.SaveChanges();
                return RedirectToAction("PostNotice");
            }
            else
            {
                return View(no);
            }
        }
        [HttpGet]
        public ActionResult UploadVideos(int id)
        {
            List<Video> con = pe.Videos.Where(x => x.Course_Id == id).ToList();
            var co = pe.Courses.Where(y => y.C_Id == id).FirstOrDefault();
            var us = pe.Users.Where(z => z.Id == co.Instructor_Id).FirstOrDefault();

            /*List<Video> cvl = con.Select(x => new Video
            {
                V_Id = x.V_Id,


            }).ToList();
            */


            return View(con);
        }

        [HttpPost]
        public ActionResult UploadVideos(HttpPostedFileBase file, int id)
        {
            var course = pe.Courses.Where(z => z.C_Id == id).FirstOrDefault();

            string FileName = Path.GetFileName(file.FileName);
            string FilePath = Path.Combine(Server.MapPath("~/Uploaded/Videos/"), FileName);

            Video content = new Video();

            content.Course_Id = course.C_Id;
            content.Video_Name = FileName;
            content.Video_Path = FilePath;
            content.Video_Description = "A small video about '" + course.C_Name + "'. ";
            file.SaveAs(FilePath);



            pe.Videos.Add(content);
            pe.SaveChanges();
            return RedirectToAction("UploadVideos");
        }


        [HttpGet]
        public ActionResult CheckMessages(int id)
        {
            Session["course_id"] = id;
            string mail = Session["email"].ToString();
            List<Email> emails = pe.Emails.Where(x => x.Couse_Id == id && x.sender != mail).ToList();
            return View(emails);
        }

        [HttpGet]
        public ActionResult CheckSentMessages(int id)
        {
            if (Session["email"] != null)
            {
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

        public ActionResult SendMessage(int id)
        {
            Email em = new Email();


            return View(em);
        }
        [HttpPost]

        public ActionResult SendMessage(Email em, int id)
        {
            em.Couse_Id = id;
            em.sender = Session["email"].ToString();

            pe.Emails.Add(em);
            pe.SaveChanges();
            return RedirectToAction("CheckMessages", "Instructor", new { @id = id });
        }


        [HttpGet]

        public ActionResult CreateMessage(int id)
        {
            Email em = new Email();


            return View(em);
        }
        [HttpPost]

        public ActionResult CreateMessage(Email em, int id)
        {
            em.Couse_Id = id;
            em.sender = Session["email"].ToString();

            pe.Emails.Add(em);
            pe.SaveChanges();
            return RedirectToAction("CheckMessages", "Instructor", new { @id = id });
        }

        [HttpGet]
        public ActionResult DeleteMessage(int id)
        {
            var email = pe.Emails.Where(x => x.id == id).FirstOrDefault();
            pe.Emails.Remove(email);
            pe.SaveChanges();
            return RedirectToAction("CheckMessages", "Instructor", new { @id = id });
        }


    }
}