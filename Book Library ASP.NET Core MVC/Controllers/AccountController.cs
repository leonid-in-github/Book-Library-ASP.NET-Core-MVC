using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Book_Library_EF_Core_Proxy_Class_Library.Proxy;
using Book_Library_EF_Core_Proxy_Class_Library.Exceptions;
using Book_Library_ASP.NET_Core_MVC;
using Book_Library_ASP.NET_Core_MVC.Models.AppConfig;
using Microsoft.Extensions.Options;
using Book_Library_ASP.NET_Core_MVC.Models.Accounts;
using Book_Library_ASP.NET_Core_MVC.Controllers;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Book_Libary_ASP.NET_Core_MVC.Controllers
{
    public class AccountController : BookLibraryController
    {
        private readonly IOptions<SessionConfig> _config;

        public AccountController(IOptions<SessionConfig> config)
        {
            _config = config;
        }

        private async Task Authenticate(string userName, int dbUserId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimTypes.NameIdentifier, dbUserId.ToString())
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private void SetupSession(int accountId, string accountLogin)
        {
            _ = Authenticate(accountLogin, accountId);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid) return View(loginModel);
            
            try
            {
                var accountId =
                    dbBookLibraryProxy.Account.Login(HttpContext.Session.Id, loginModel.Login, loginModel.Password);
                if (accountId == 0)
                {
                    ModelState.AddModelError("LoginMassege", "Login failed. Incorrect login or password.");
                    return View();
                }

                SetupSession(accountId, loginModel.Login);

                return RedirectToAction("Index", "Home");
            }
            catch (SessionExpirationConflictException)
            {
                if (Request.Cookies[_config.Value.SessioCookieName] != null)
                {
                    Response.Cookies.Append(_config.Value.SessioCookieName, "", new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    });
                }
                ModelState.AddModelError("LoginMassege", "Retry.");
            }
            catch (Exception)
            {

            }
            return View(loginModel);
        }

        public IActionResult Logout()
        {
            LogoutApplication();

            return RedirectToAction("Index", "Home");
        }

        public void LogoutApplication()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                dbBookLibraryProxy.Account.Logout(HttpContext.Session.Id);

                HttpContext.Session.Clear();
                if (Request.Cookies[_config.Value.SessioCookieName] != null)
                {
                    Response.Cookies.Append(_config.Value.SessioCookieName, "", new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    });
                }
            }
            catch (Exception) { }
            return;
        }

        public IActionResult Registration()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Registration(RegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var accountId =
                        dbBookLibraryProxy.Account.Register(HttpContext.Session.Id, registrationModel.Login, registrationModel.Password,
                        registrationModel.FirstName, registrationModel.LastName, registrationModel.Email);

                    if (accountId == -1)
                    {
                        ModelState.AddModelError("RegistrationMassege", "Account already exists.");
                        return View();
                    }

                    SetupSession(accountId, registrationModel.Login);

                    return RedirectToAction("Index", "Home");
                }
                catch (SessionExpirationConflictException)
                {
                    if (Request.Cookies[_config.Value.SessioCookieName] != null)
                    {
                        Response.Cookies.Append(_config.Value.SessioCookieName, "", new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        });
                    }
                    ModelState.AddModelError("RegistrationMassege", "Retry.");
                }
                catch (Exception)
                {

                }
            }
            return View();
        }

        public IActionResult GetUser()
        {
            if (!IsLoged) return RedirectToAction("Index", "Home");
            try
            {
                if(Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                {
                    var model = (UserModel)dbBookLibraryProxy.Account.GetUser((int)aId);
                    return View(model);
                }
            }
            catch (Exception) { }

            return new EmptyResult();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (!IsLoged) return RedirectToAction("Index", "Home");
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        var result = dbBookLibraryProxy.Account.ChangeAccountPassword((int)aId, model.Password, model.NewPassword);

                        if (result)
                            return RedirectToAction("GetUser", "Account");
                        else
                        {
                            ModelState.AddModelError("ChangePasswordMassege", "Change password failed. Incorrect data.");
                            return View();
                        }
                    }
                }
                catch (Exception) { }
            }
            return View(model);
        }

        public IActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteAccount(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                if (!IsLoged) return RedirectToAction("Index", "Home");
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        var result = dbBookLibraryProxy.Account.DeleteAccount((int)aId, model.Password);
                        if (result)
                        {
                            LogoutApplication();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("DeleteAccountMassege", "Delete account failed. Incorrect password.");
                            return View();
                        }
                    }
                }
                catch (Exception) { }
            }
            return View();
        }
    }
}