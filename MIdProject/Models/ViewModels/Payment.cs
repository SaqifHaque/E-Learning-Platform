using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class Payment
    {

        [StringLength(10, MinimumLength = 10, ErrorMessage = "This field must be 10 characters")]
        public string Transaction { set; get; }
    }
}