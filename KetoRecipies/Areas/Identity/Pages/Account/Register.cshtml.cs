using KetoRecipies.Models;
using KetoRecipies.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KetoRecipies.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            public string honey { get; set; }
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Facebook URL")]
            public string Facebook { get; set; }

            [Display(Name = "YouTube URL")]
            public string YouTube { get; set; }

            [Display(Name = "Instagram URL")]
            public string Instagram { get; set; }

            [Display(Name = "Twitter URL")]
            public string Twitter { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = Input.Name, UserName = Input.Email, Email = Input.Email, Facebook = Input.Facebook, YouTube = Input.YouTube, Instagram = Input.Instagram, Twitter = Input.Twitter };
                user.RegistrationDate = DateTime.Now;
                ApplicationUser checkForDupe = _userManager.Users.FirstOrDefault(u => u.Email == Input.Email);
                if(checkForDupe != null)
                {
                    TempData["UserExists"] = "Sorry this email address is already in use, please use another email address or login to your current account.";
                    return Page();
                }
                if (string.IsNullOrEmpty(Input.honey))
                {
                    int code = GenerateVerificationCodeHelper();
                    await _emailSender.SendEmailAsync(user.Email, "Confirm Your Email Address", $"Your 4 digit verification code is {code}");

                    VerifyEmailViewModel ve = new VerifyEmailViewModel();
                    ve.VerificationCode = code;                  
                    ve.Password = Input.Password;
                    ve.Email = Input.Email;
                    ve.JSONUser = JsonConvert.SerializeObject(user);

                    return RedirectToAction("ConfirmEmail", "Home", new RouteValueDictionary(ve));
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        public int GenerateVerificationCodeHelper()
        {
            int min = 1000;
            int max = 9999;
            Random verificationCode = new Random();

            return verificationCode.Next(min, max);
        }
    }
}
