using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public String Instructor_Name { get; set; }
        public String SubjectName { get; set; }
        public string C_Name { get; set; }
        public string C_Description { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
    }

}