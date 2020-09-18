using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Provide Email")]
        

        public string Email { get; set; }

        [Required(ErrorMessage = "Please Provide Password")]
        public string Password { get; set; }
    }
}