//Mục đích quản lý chỉnh sửa thông tin cá nhân seeker
//[HttpPost] hàm get thông tin từ view qua model -> controller
//[HttpGet] Hàm load thông tin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MEMO_JOB.Models;
using MEMO_JOB.Models.Recruiters;
using MEMO_JOB.Models.Recruiters.ManageViewModels;
using MEMO_JOB.Services;
using Microsoft.EntityFrameworkCore;
using MEMO_JOB.Extensions;
using MEMO_JOB.Models.Recruiters.AccountViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Routing;

namespace MEMO_JOB.Controllers.Recruiters
{
    [Authorize(Roles = "Recruiter")]
    [Route("[controller]/[action]")]
    public class ManageJobRecruitersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        public MEMO_JOBContext _context;
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageJobRecruitersController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageJobRecruitersController> logger,
          UrlEncoder urlEncoder,
          MEMO_JOBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> RecruiterAccountManagement(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var GetCompany = _context.AppJobRecruitersCompany.Where(c => c.CompanyIdd == user.CompanyIdd).FirstOrDefault();
            var ListUser = _context.AppJobUsers.Where(c => c.CompanyIdd == user.CompanyIdd).OrderBy(b => b.DateCreated).ToList();
            if (user == null || GetCompany == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            ViewData["ReturnUrl"] = returnUrl;
            var modelList = new List<RecruiterAccountManagementViewModel>();

            for (int n = 0; n < ListUser.Count(); n++)
            {
                //string newsUrl = new Uri(Request.Url.Scheme + "://" + Request.Url.Host + ":3153/News/Index/" + n).ToString();
                var item = ListUser[n];

                var model = new RecruiterAccountManagementViewModel()
                {
                    Email = (item.Email).Substring(15),
                    FullName = item.FullName,
                    UserId = item.Id,
                    AccountType = item.Discriminator,
                    DateCreated = item.DateCreated.ToString(),
                    Position = item.Position,
                    Subscribe = item.Subscribe,
                    PhoneNumber = item.PhoneNumber,
                    Active = item.LockoutEnabled
                };
                modelList.Add(model);
            }

            return View(modelList);
        }
        //Đăng ký tài khoản phụ nhà tuyển dụng
        [HttpGet]
        public async Task<IActionResult> SubRegister(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var CountUser = _context.AppJobUsers.Where(f => f.CompanyIdd == user.CompanyIdd).ToList();

            var FindCompany = _context.AppJobRecruitersCompany.Where(v => v.CompanyIdd == user.CompanyIdd).FirstOrDefault();
            if (CountUser.Count() > FindCompany.CompanyMaxUsers)
            {
                ModelState.AddModelError(string.Empty, "Số tài khoản cho phép của công ty này là :" + FindCompany.CompanyMaxUsers);
                ViewData["ReturnUrl"] = returnUrl;
                //return RedirectToAction(nameof(RecruiterAccountManagement));
                return RedirectToAction("RecruiterAccountManagement", new RouteValueDictionary(
               new { controller = "ManageJobRecruiters", action = "RecruiterAccountManagement", returnUrl = FindCompany.CompanyMaxUsers }));
            }
            else
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> SubRegister(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var CheckUserExist = await _userManager.FindByEmailAsync("IRecruiterUserI" + model.Email);
                var CountUser = _context.AppJobUsers.Where(f => f.CompanyIdd == user.CompanyIdd).ToList();

                @ViewData["Message"] = "Model valid";

                if (CheckUserExist == null)
                {
                    var FindCompany = _context.AppJobRecruitersCompany.Where(v => v.CompanyIdd == user.CompanyIdd).FirstOrDefault();
                    if (CountUser.Count() > FindCompany.CompanyMaxUsers)
                    {
                        ModelState.AddModelError(string.Empty, "Số tài khoản cho phép của công ty này là :" + FindCompany.CompanyMaxUsers);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email đã đăng ký trên hệ thống");
                    return View(model);
                }

                var SubUser = new SubRecruiterUser
                {
                    FullName = model.FullName,
                    UserName = "IRecruiterUserI" + model.Email,
                    Email = "IRecruiterUserI" + model.Email,
                    NormalizedUserName = "IRecruiterUserI" + model.Email,
                    NormalizedEmail = "IRecruiterUserI" + model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    PasswordChangeDate = DateTime.Now,
                    Birthday = Convert.ToDateTime("01/01/2000"),
                    //SecurityStamp = Guid.NewGuid().ToString()
                    PhoneDesk = model.PhoneDesk,
                    Fax = model.Fax,
                    Address = model.Address,
                    City = model.City,
                    Nation = model.Nation,
                    Position = model.Position,
                    PublishFrom = DateTime.Now,
                    PublishTo = DateTime.Now,
                    CompanyIdd = Convert.ToInt64(user.CompanyIdd),
                };
                var result = await _userManager.CreateAsync(SubUser, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SubRecruiter");

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
        [HttpGet]
        public async Task<IActionResult> SubRegisterEdit(string id)
        {
            var GetUser = await _context.AppJobUsers.Where(c => c.Id == id).FirstOrDefaultAsync();

            var model = new IndexViewModel
            {
                UserId = GetUser.Id,
                Username = (GetUser.UserName).Substring(15),
                FullName = GetUser.FullName,
                Email = (GetUser.Email).Substring(15),
                PhoneNumber = GetUser.PhoneNumber,
                PhoneDesk = GetUser.PhoneDesk,
                Position = GetUser.Position,
                Fax = GetUser.Fax,
                IsEmailConfirmed = GetUser.EmailConfirmed,
                Address = GetUser.Address,
                Subscribe = GetUser.Subscribe,
                City = GetUser.City,
                Nation = GetUser.Nation,
                Active = GetUser.LockoutEnabled,

                StatusMessage = StatusMessage
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubRegisterEdit(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var GetUser = _context.AppJobUsers.Where(c => c.Id == model.UserId).FirstOrDefault();
            if (GetUser == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var FullName = GetUser.FullName;
            if (model.FullName != FullName)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.FullName))
                {
                    try
                    {
                        GetUser.FullName = model.FullName;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Address = GetUser.Address;
            if (model.Address != Address)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.Address))
                {
                    try
                    {
                        GetUser.Address = model.Address;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var City = GetUser.City;
            if (model.City != City)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.City))
                {
                    try
                    {
                        GetUser.City = model.City;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Nation = GetUser.Nation;
            if (model.Nation != Nation)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.Nation))
                {
                    try
                    {
                        GetUser.Nation = model.Nation;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Email = GetUser.Email;
            if (model.Email != Email)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.Email, s => s.UserName, s => s.NormalizedUserName, s => s.NormalizedEmail))
                {
                    try
                    {
                        GetUser.Email = "IRecruiterUserI" + model.Email;
                        GetUser.UserName = "IRecruiterUserI" + model.Email;
                        GetUser.NormalizedUserName = "IRecruiterUserI" + model.Email;
                        GetUser.NormalizedEmail = "IRecruiterUserI" + model.Email;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var PhoneNumber = GetUser.PhoneNumber;
            if (model.PhoneNumber != PhoneNumber)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.PhoneNumber))
                {
                    try
                    {
                        GetUser.PhoneNumber = model.PhoneNumber;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var PhoneDesk = GetUser.PhoneDesk;
            if (model.PhoneDesk != PhoneDesk)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.PhoneDesk))
                {
                    try
                    {
                        GetUser.PhoneDesk = model.PhoneDesk;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Fax = GetUser.Fax;
            if (model.Fax != Fax)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.Fax))
                {
                    try
                    {
                        GetUser.Fax = model.Fax;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Position = GetUser.Position;
            if (model.Position != Position)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.Position))
                {
                    try
                    {
                        GetUser.Position = model.Position;
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var Active = GetUser.LockoutEnabled;
            if (model.Active != Active)
            {
                if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.LockoutEnabled))
                {
                    try
                    {
                        GetUser.LockoutEnabled = model.Active;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            if (await TryUpdateModelAsync<AppJobUsers>(GetUser, "", s => s.DateUpdated))
            {
                try
                {
                    GetUser.DateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            StatusMessage = "Your profile has been updated";
            return RedirectToAction("SubRegisterEdit", new RouteValueDictionary(
                new { controller = "ManageJobRecruiters", action = "SubRegisterEdit", id = GetUser.Id }));
        }

        [HttpGet]
        public async Task<IActionResult> CompanyIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            var GetCompany = _context.AppJobRecruitersCompany.Where(c => c.CompanyIdd == user.CompanyIdd).FirstOrDefault();
            if (user == null || GetCompany == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new CompanyIndexViewModel
            {
                CompanyName = GetCompany.CompanyName,
                CompanyBanner = GetCompany.CompanyBanner,
                CompanyImage = GetCompany.CompanyImage,
                CompanyLogo = GetCompany.CompanyInfo,
                CompanyInfo = GetCompany.CompanyInfo,
                CompanySize = GetCompany.CompanySize,
                CompanyTaxcode = GetCompany.CompanyTaxcode,
                CompanyType = GetCompany.CompanyType,
                CompanyWebsite = GetCompany.CompanyWebsite,

                StatusMessage = StatusMessage
            };

            return View(model);
        }
        //Hàm sửa thông tin cá nhân Recruiter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyIndex(CompanyIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            var GetCompany = _context.AppJobRecruitersCompany.Where(c => c.CompanyIdd == user.CompanyIdd).FirstOrDefault();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var CompanyBanner = GetCompany.CompanyBanner;
            if (model.CompanyBanner != CompanyBanner)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyBanner))
                {
                    try
                    {
                        GetCompany.CompanyBanner = model.CompanyBanner;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyName = GetCompany.CompanyName;
            if (model.CompanyName != CompanyName)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyName))
                {
                    try
                    {
                        GetCompany.CompanyName = model.CompanyName;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanySize = GetCompany.CompanySize;
            if (model.CompanySize != CompanySize)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanySize))
                {
                    try
                    {
                        GetCompany.CompanySize = model.CompanySize;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyType = GetCompany.CompanyType;
            if (model.CompanyType != CompanyType)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyType))
                {
                    try
                    {
                        GetCompany.CompanyType = model.CompanyType;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyInfo = GetCompany.CompanyInfo;
            if (model.CompanyInfo != CompanyInfo)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyInfo))
                {
                    try
                    {
                        GetCompany.CompanyInfo = model.CompanyInfo;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyImage = GetCompany.CompanyImage;
            if (model.CompanyImage != CompanyImage)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyImage))
                {
                    try
                    {
                        GetCompany.CompanyImage = model.CompanyImage;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyLogo = GetCompany.CompanyLogo;
            if (model.CompanyLogo != CompanyLogo)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyLogo))
                {
                    try
                    {
                        GetCompany.CompanyLogo = model.CompanyLogo;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyTaxcode = GetCompany.CompanyTaxcode;
            if (model.CompanyTaxcode != CompanyTaxcode)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyTaxcode))
                {
                    try
                    {
                        GetCompany.CompanyTaxcode = model.CompanyTaxcode;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            var CompanyWebsite = GetCompany.CompanyWebsite;
            if (model.CompanyWebsite != CompanyWebsite)
            {
                if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyWebsite))
                {
                    try
                    {
                        GetCompany.CompanyWebsite = model.CompanyWebsite;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            }
            if (await TryUpdateModelAsync<AppJobRecruitersCompany>(GetCompany, "", s => s.CompanyDateUpdated))
            {
                try
                {
                    GetCompany.CompanyDateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(CompanyIndex));
        }

        //Load thông tin cá nhân Recuiter
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = (user.UserName).Substring(15),
                FullName = user.FullName,
                Email = (user.Email).Substring(15),
                PhoneNumber = user.PhoneNumber,
                PhoneDesk = user.PhoneDesk,
                Position = user.Position,
                Fax = user.Fax,
                IsEmailConfirmed = user.EmailConfirmed,
                Address = user.Address,
                Subscribe = user.Subscribe,
                City = user.City,
                Nation = user.Nation,

                StatusMessage = StatusMessage
            };

            return View(model);
        }
        //Hàm sửa thông tin cá nhân Recruiter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            user.DateUpdated = DateTime.Now;

            var email = user.Email;
            if ("IRecruiterUserI" + model.Email != email)
            {
                var CheckExistEmail = await _userManager.FindByEmailAsync(model.Email);

                if (CheckExistEmail == null)
                {
                    user.Email = "IRecruiterUserI" + model.Email;
                    user.NormalizedEmail = "IRecruiterUserI" + model.Email;
                    user.NormalizedUserName = "IRecruiterUserI" + model.Email;
                    user.UserName = "IRecruiterUserI" + model.Email;

                    var setEmailResult = await _userManager.UpdateAsync(user);
                    if (!setEmailResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email đã có người đăng ký.");
                    return View(model);
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
            var FullName = user.FullName;
            if (model.FullName != FullName)
            {
                user.FullName = model.FullName;
                var setFirstNameResult = await _userManager.UpdateAsync(user);
                if (!setFirstNameResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting full name for user with ID '{user.Id}'.");
                }
            }

            var PhoneDesk = user.PhoneDesk;
            if (model.PhoneDesk != PhoneDesk)
            {
                user.PhoneDesk = model.PhoneDesk;
                var setPhoneDeskResult = await _userManager.UpdateAsync(user);
                if (!setPhoneDeskResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting address for user with ID '{user.Id}'.");
                }
            }
            var Position = user.Position;
            if (model.Position != Position)
            {
                user.Position = model.Position;
                var setPositionResult = await _userManager.UpdateAsync(user);
                if (!setPositionResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting address for user with ID '{user.Id}'.");
                }
            }
            var Fax = user.Fax;
            if (model.Fax != Fax)
            {
                user.Fax = model.Fax;
                var setFaxResult = await _userManager.UpdateAsync(user);
                if (!setFaxResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting address for user with ID '{user.Id}'.");
                }
            }
            var Address = user.Address;
            if (model.Address != Address)
            {
                user.Address = model.Address;
                var setAddressResult = await _userManager.UpdateAsync(user);
                if (!setAddressResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting address for user with ID '{user.Id}'.");
                }
            }

            var City = user.City;
            if (model.City != City)
            {
                user.City = model.City;
                var setCityResult = await _userManager.UpdateAsync(user);
                if (!setCityResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting last name for user with ID '{user.Id}'.");
                }
            }

            var Nation = user.Nation;
            if (model.Nation != Nation)
            {
                user.Nation = model.Nation;
                var setNationResult = await _userManager.UpdateAsync(user);
                if (!setNationResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting nation for user with ID '{user.Id}'.");
                }
            }

            var Subscribe = user.Subscribe;
            if (model.Subscribe != Subscribe)
            {
                user.Subscribe = model.Subscribe;
                var setSubscribeResult = await _userManager.UpdateAsync(user);
                if (!setSubscribeResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting subscribe for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Lấy token xác nhận tài khoản
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Gọi UrlHelperExtensions.cs lấy được chuỗi dẫn đến AccountController.ConfirmEmail
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            // Gửi mail chuỗi callbackUrl gọi hàm AccountController.ConfirmEmail
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"'{_userManager.GetUserId(User)}' không tồn tại.");
            }
            user.PasswordChangeDate = DateTime.Now;

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LinkLogin(string provider)
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    // Request a redirect to the external login provider to link a login for the current user
        //    var redirectUrl = Url.Action(nameof(LinkLoginCallback), "ManageJobSeekers");
        //    //var redirectUrl = Url.Action(nameof(LinkLoginCallback));
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
        //    return new ChallengeResult(provider, properties);
        //}

        //[HttpGet]
        //public async Task<IActionResult> LinkLoginCallback()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
        //    }

        //    var result = await _userManager.AddLoginAsync(user, info);
        //    if (!result.Succeeded)
        //    {
        //        throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
        //    }

        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //    StatusMessage = "The external login was added.";
        //    var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
        //    return RedirectToAction(nameof(ManageLogins), new { Message = message });
        //    //return RedirectToAction(nameof(ExternalLogins));
        //}

        //GET: /Manage/ManageLogins
        //[HttpGet]
        //public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        //{
        //    ViewData["StatusMessage"] =
        //        message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    var user = await GetCurrentUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userLogins = await _userManager.GetLoginsAsync(user);
        //    var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        //    var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();
        //    ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
        //    return View(new ManageLoginsViewModel
        //    {
        //        CurrentLogins = userLogins,
        //        OtherLogins = otherLogins
        //    });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("MEMO_JOB"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
