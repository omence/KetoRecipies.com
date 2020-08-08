using System.ComponentModel.DataAnnotations;

namespace KetoRecipies.Models
{
    public class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Does Not Match")]
        public string ConfirmPassword { get; set; }
    }
}
