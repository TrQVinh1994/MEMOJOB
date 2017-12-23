﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MEMO_JOB.Models;
using MEMO_JOB.Models.JobSeekers;
using MEMO_JOB.Models.JobSeekers.AccountViewModels;
using MEMO_JOB.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using MEMO_JOB.Extensions;

namespace MEMO_JOB.Controllers.JobSeekers
{
    [Authorize]
    [Route("JobSeekers/[controller]/[action]")]
    public class AccountJobSeekersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        public MEMO_JOBContext _context;

        public AccountJobSeekersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountJobSeekersController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] // The attribute validates the hidden XSRF token generated by the anti-forgery token generator in the Form Tag Helper
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await _userManager.FindByEmailAsync("IJobSeekerUserI" + model.Email);
                if (user != null)
                {

                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Vui lòng xác nhận email của bạn.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại trong hệ thống.");
                }
                //Chống tấn công Open Redirection
                //if (Url.IsLocalUrl(returnUrl))
                //{

                //    return Redirect(returnUrl);

                //}
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync("IJobSeekerUserI" + model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập thất bại.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                @ViewData["Message"] = "Model valid";
                var user = new JobSeekerUser
                {
                    FullName = model.FullName,
                    UserName = "IJobSeekerUserI" + model.Email,
                    Email = "IJobSeekerUserI" + model.Email,
                    NormalizedUserName = "IJobSeekerUserI" + model.Email,
                    NormalizedEmail = "IJobSeekerUserI" + model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    PasswordChangeDate = DateTime.Now,
                    Birthday = Convert.ToDateTime("01/01/2000")
                    //SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Seeker");

                    _logger.LogInformation("User created a new account with password.");

                    // Lấy token xác nhận tài khoản
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Gọi UrlHelperExtensions.cs lấy được chuỗi dẫn đến AccountController.ConfirmEmail
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                    // Gửi mail chuỗi callbackUrl gọi hàm AccountController.ConfirmEmail
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    //await _signInManager.SignInAsync(user, isPersistent: false); //isPersistent 
                    _logger.LogInformation("User created a new account with password.");
                    //return RedirectToLocal(returnUrl);
                    ViewData["EmailConfirm"] = model.Email;
                    return View("SendingEmailConfirm", new SendingEmailConfirmViewModel
                    {
                        Email = model.Email,
                    });
                }
                AddErrors(result);
            }
            else
            {
                @ViewData["Message"] = "Invalid!!!";
            }
            //Định nghĩa isPersistent 
            //Persistent cookies will be saved as files in the browser folders until they either expire or manually deleted. 
            //This will cause the cookie to persist even if you close the browser.
            //If IsPersistent is set to false, the browser will acquire session cookie which gets cleared when the browser is closed

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult SendingEmailConfirm(string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendingEmailConfirm(SendingEmailConfirmViewModel model, string returnUrl = null)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user.");
        //    }
        //    if (!await _userManager.IsEmailConfirmedAsync(user))
        //    {
        //        var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
        //        if (!setEmailResult.Succeeded)
        //        {
        //            throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
        //        }
        //        else
        //        {
        //            // Lấy token xác nhận tài khoản
        //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //            // Gọi UrlHelperExtensions.cs lấy được chuỗi dẫn đến AccountController.ConfirmEmail
        //            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

        //            // Gửi mail chuỗi callbackUrl gọi hàm AccountController.ConfirmEmail
        //            await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

        //            ModelState.AddModelError(string.Empty, "Link xác nhận đã gửi đến " + model.Email + ". Vui lòng xác nhận email của bạn.");
        //            return View(model);
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Email đã được kích hoạt");
        //        return View(model);
        //    }
        //    //return RedirectToAction(nameof(Login));

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeJobSeekersController.Index), "HomeJobSeekers");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //Login bên ngoài
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "AccountJobSeekers", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            //info.Principal //the IPrincipal with the claims from facebook
            //info.ProviderKey //an unique identifier from Facebook for the user that just signed in
            //info.LoginProvider //a string with the external login provider name, in this case Facebook
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                ViewData["Email"] = email;

                return View("ExternalLogin", new ExternalLoginViewModel
                {
                    Email = email,
                    FullName = name,
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var Checkemail = await _userManager.FindByEmailAsync(model.Email);
                if (Checkemail != null)
                {
                    var result1 = await _userManager.AddLoginAsync(Checkemail, info);
                    if (!result1.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{Checkemail.Id}'.");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(Checkemail, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {

                    //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var user = new JobSeekerUser
                    {
                        FullName = model.FullName,
                        UserName = model.Email,
                        Email = model.Email,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        LastLoginDate = DateTime.Now,
                        PasswordChangeDate = DateTime.Now,
                        EmailConfirmed = true,
                        Birthday = Convert.ToDateTime("01/01/2000"),
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeJobSeekersController.Index), "HomeJobSeekers");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
                // throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //Lấy token của user
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Gọi UrlHelperExtensions.cs lấy được chuỗi dẫn đến AccountController.ResetPassword
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                // Gửi mail chuỗi callbackUrl gọi hàm AccountController.ResetPassword
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeJobSeekersController.Index), "HomeJobSeekers");
            }
        }

        #endregion
    }
}
