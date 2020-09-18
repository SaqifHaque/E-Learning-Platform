using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MIdProject.Models.ViewModels
{
    public class EditProfileViewModel
    {
        //[Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Provide Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Provide Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Provide Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please Provide Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Please Provide Password")]
        public string Password { get; set; }
    }
}