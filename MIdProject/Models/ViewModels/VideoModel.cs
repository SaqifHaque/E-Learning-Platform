using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class VideoModel
    {
        public int V_Id { get; set; }
        public int Course_Id { get; set; }
        public int Course_Name { get; set; }
        public int Instructor_Name { get; set; }
        public string Video_Name { get; set; }
        public string Video_Description { get; set; }
        public string Video_Path { get; set; }
    }
}