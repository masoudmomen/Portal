using Microsoft.AspNetCore.Identity;
using Portal.Data;

namespace Portal.Services
{
    public class AuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> LoginAsync(string userNameOrEmail, string password, bool rememberMe)
        {
            ApplicationUser? user;

            if (userNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(userNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(userNameOrEmail);
            }

            if (user == null)
            {
                return (false, "کاربری با این مشخصات یافت نشد.");
            }

            if (!user.IsActive)
            {
                return (false, "حساب کاربری غیرفعال است.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return (true, string.Empty);
            }

            return (false, "نام کاربری یا رمز عبور نادرست است.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
