using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMDesktopUI.Library.Models
{
    public class CreateUserModel
    {   [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [DisplayName("Email Adress")]
        public string EmailAdress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        [Compare(nameof(Password), ErrorMessage ="The password do not match")]
        public string ConfirmPassword { get; set; }

    }
}
