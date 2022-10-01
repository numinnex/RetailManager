using System.ComponentModel.DataAnnotations;

namespace RMApi.Models
{
    public class CreateUserModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAdress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage ="The password do not match")]
        public string ConfirmPassword { get; set; }
        
    }
}
