using BookLibrary.Storage.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.WebServer.Models.Accounts
{
    public class UserModel
    {
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public static explicit operator UserModel(User model)
        {
            return new UserModel { Login = model.Login, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };
        }
    }
}
