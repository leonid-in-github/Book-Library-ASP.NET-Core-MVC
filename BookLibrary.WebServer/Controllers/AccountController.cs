using BookLibrary.Repository.Exceptions;
using BookLibrary.Repository.Repositories;
using BookLibrary.WebServer.AppConfig;
using BookLibrary.WebServer.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Book_Libary_ASP.NET_Core_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDataStore dataStore;

        private readonly IOptions<SessionConfig> _config;

        public AccountController(IOptions<SessionConfig> config, IDataStore dataStore)
        {
            _config = config;
            this.dataStore = dataStore;

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
                    dataStore.Account.Login(HttpContext.Session.Id, loginModel.Login, loginModel.Password);
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
                if (Request.Cookies[_config.Value.SessionCookieName] != null)
                {
                    Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    });
                }
                ModelState.AddModelError("LoginMassege", "Retry.");
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
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            dataStore.Account.Logout(HttpContext.Session.Id);

            HttpContext.Session.Clear();
            if (Request.Cookies[_config.Value.SessionCookieName] != null)
            {
                Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
            }
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
                        dataStore.Account.Register(HttpContext.Session.Id, registrationModel.Login, registrationModel.Password,
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
                    if (Request.Cookies[_config.Value.SessionCookieName] != null)
                    {
                        Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        });
                    }
                    ModelState.AddModelError("RegistrationMassege", "Retry.");
                }
            }
            return View();
        }

        [Authorize]
        public IActionResult GetUser()
        {
            if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
            {
                var model = (UserModel)dataStore.Account.GetUser((int)aId);
                return View(model);
            }

            return new EmptyResult();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                {
                    var result = dataStore.Account.ChangeAccountPassword((int)aId, model.Password, model.NewPassword);

                    if (result)
                        return RedirectToAction("GetUser", "Account");
                    else
                    {
                        ModelState.AddModelError("ChangePasswordMassege", "Change password failed. Incorrect data.");
                        return View();
                    }
                }
            }
            return View(model);
        }

        public IActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteAccount(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                {
                    var result = dataStore.Account.DeleteAccount((int)aId, model.Password);
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
            return View();
        }
    }
}