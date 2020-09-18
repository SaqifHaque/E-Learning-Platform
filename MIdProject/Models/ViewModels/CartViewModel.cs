using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class CartViewModel
    {
        public int Cart_Id { get; set; }
        public int Student_Id { get; set; }
        public int Item_Id { get; set; }
        public string C_Name { get; set; }

        public double Price { get; set; }
    }
}