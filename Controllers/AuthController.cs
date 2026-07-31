using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Portal.Data;
using System.ComponentModel.DataAnnotations;

namespace Portal.Controllers
{
    [AllowAnonymous]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToLoginWithError("اطلاعات ورود نامعتبر است.", model.ReturnUrl);
            }

            var user = model.UserNameOrEmail.Contains("@")
                ? await _userManager.FindByEmailAsync(model.UserNameOrEmail)
                : await _userManager.FindByNameAsync(model.UserNameOrEmail);

            if (user == null)
            {
                return RedirectToLoginWithError("نام کاربری یا رمز عبور اشتباه است.", model.ReturnUrl);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("/");
            }

            if (result.IsLockedOut)
            {
                return RedirectToLoginWithError("حساب کاربری موقتاً قفل شده است.", model.ReturnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToLoginWithError("ورود دومرحله‌ای برای این حساب فعال است.", model.ReturnUrl);
            }

            return RedirectToLoginWithError("نام کاربری یا رمز عبور اشتباه است.", model.ReturnUrl);
        }

        [Authorize]
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/login");
        }

        private IActionResult RedirectToLoginWithError(string error, string? returnUrl)
        {
            var query = new Dictionary<string, string?>()
            {
                ["error"] = error,
                ["returnUrl"] = returnUrl
            };

            var loginUrl = QueryHelpers.AddQueryString("/login",
                query.Where(x => !string.IsNullOrWhiteSpace(x.Value))!
                     .ToDictionary(x => x.Key, x => x.Value));

            return Redirect(loginUrl);
        }

        public class LoginPostModel
        {
            [Required]
            public string UserNameOrEmail { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }

            public string? ReturnUrl { get; set; }
        }
    }
}
