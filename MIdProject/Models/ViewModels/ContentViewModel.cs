using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class ContentViewModel
    {
        public string Course_Name { set; get; }

        public int Course_Id { set; get; }

        public string Instructor_Name { set; get; }

        public string File_Name { set; get; }
        public string File_Path { set; get; }
    }
}