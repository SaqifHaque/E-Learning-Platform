using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIdProject.Models.ViewModels
{
    public class CourseEditModel
    {
        [Key]
        public int C_Id { get; set; }
        public int Instructor_Id { get; set; }
        public int Subject_Id { get; set; }

        public String Instructor_Name { get; set; }
        public String SubjectName { get; set; }

        [Required(ErrorMessage = "Course Name required")]
        public string C_Name { get; set; }

        [Required(ErrorMessage = "Please, Add Description")]
        public string C_Description { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Please,Provide Amount")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Please,Add An Image")]
        public string C_Image { get; set; }

        public List<User> InstructorList;
        public SelectList instructors { get; set; }
        public string SelectInstructor { get; set; }

        public List<Subject> CatList;
        public SelectList Category { get; set; }
        public string SelectCategory { get; set; }
    }
}