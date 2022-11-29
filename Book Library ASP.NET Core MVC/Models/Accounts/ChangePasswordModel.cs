using System.ComponentModel.DataAnnotations;

namespace BookLibrary.WebServer.Models.Accounts
{
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 32 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 32 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 32 characters.")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        public string ChangePasswordMassege { get; set; }
    }
}
