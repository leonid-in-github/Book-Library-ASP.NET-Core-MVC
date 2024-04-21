using BookLibrary.Storage.Exceptions;
using BookLibrary.Storage.Repositories;
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
        private readonly IAccountRepository accountRepository;
        private readonly IOptions<SessionConfig> _config;

        public AccountController(IOptions<SessionConfig> config, IAccountRepository accountRepository)
        {
            _config = config;
            this.accountRepository = accountRepository;

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid) return View(loginModel);

            try
            {
                var sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("sessionId", sessionId);
                var accountId =
                    await accountRepository.Login(sessionId, loginModel.Login, loginModel.Password);
                if (accountId == 0)
                {
                    ModelState.AddModelError("LoginMassege", "Login failed. Incorrect login or password.");
                    return View();
                }

                SetupSession(accountId, loginModel.Login);

                return RedirectToAction("Index", "Home");
            }
            catch (SessionExpirationException)
            {
                if (Request.Cookies[_config.Value.SessionCookieName] != null)
                {
                    Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                    {
                        Expires = DateTime.UtcNow.AddDays(-1)
                    });
                }
                ModelState.AddModelError("LoginMassege", "Retry.");
            }
            return View(loginModel);
        }

        public async Task<IActionResult> Logout()
        {
            await LogoutApplication();

            return RedirectToAction("Index", "Home");
        }

        public async Task LogoutApplication()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var sessionId = Request.Cookies["sessionId"];
            await accountRepository.Logout(sessionId);

            HttpContext.Session.Clear();
            if (Request.Cookies[_config.Value.SessionCookieName] != null)
            {
                Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                {
                    Expires = DateTime.UtcNow.AddDays(-1)
                });
            }
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sessionId = Guid.NewGuid().ToString();
                    Response.Cookies.Append("sessionId", sessionId);
                    var accountId =
                        await accountRepository.Register(sessionId, registrationModel.Login, registrationModel.Password,
                        registrationModel.FirstName, registrationModel.LastName, registrationModel.Email);

                    if (accountId == -1)
                    {
                        ModelState.AddModelError("RegistrationMassege", "Account already exists.");
                        return View();
                    }

                    SetupSession(accountId, registrationModel.Login);

                    return RedirectToAction("Index", "Home");
                }
                catch (SessionExpirationException)
                {
                    if (Request.Cookies[_config.Value.SessionCookieName] != null)
                    {
                        Response.Cookies.Append(_config.Value.SessionCookieName, "", new CookieOptions()
                        {
                            Expires = DateTime.UtcNow.AddDays(-1)
                        });
                    }
                    ModelState.AddModelError("RegistrationMassege", "Retry.");
                }
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
            {
                var model = (UserModel)await accountRepository.GetUser((int)aId);
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
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                {
                    var result = await accountRepository.ChangeAccountPassword((int)aId, model.Password, model.NewPassword);

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
        public async Task<IActionResult> DeleteAccount(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                {
                    var result = await accountRepository.DeleteAccount((int)aId, model.Password);
                    if (result)
                    {
                        await LogoutApplication();
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

        #region Private

        private async Task Authenticate(string userName, int dbUserId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimTypes.NameIdentifier, dbUserId.ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private void SetupSession(int accountId, string accountLogin)
        {
            _ = Authenticate(accountLogin, accountId);
        }

        #endregion
    }
}