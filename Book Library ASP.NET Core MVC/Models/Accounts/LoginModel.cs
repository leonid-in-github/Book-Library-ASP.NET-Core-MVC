using System.ComponentModel.DataAnnotations;

namespace Book_Library_ASP.NET_Core_MVC.Models.Accounts
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 32 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string LoginMassege { get; set; }
    }
}
