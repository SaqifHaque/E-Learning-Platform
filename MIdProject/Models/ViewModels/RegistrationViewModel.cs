using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class RegistrationViewModel
    {
     
        
        [Required(ErrorMessage ="Please provide a Name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Please provide a Email")]
        public string Email { get; set; }
        
     
        public string Type { get; set; }
        

        public string Status { get; set; }
        
        [Required(ErrorMessage = "Please provide a Password")]
        public string Password { get; set; }
    }
}