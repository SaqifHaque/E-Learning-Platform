//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MIdProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Email
    {
        public int id { get; set; }
        public int Couse_Id { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string message { get; set; }
    
        public virtual Cours Cours { get; set; }
    }
}