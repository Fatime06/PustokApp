using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterVM userRegisterVm)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser user = await _userManager.FindByNameAsync(userRegisterVm.UserName);
            if (user != null)
            {
                ModelState.AddModelError("UserName", "This username already exists");
                return View();
            }

            user = new AppUser
            {
                UserName = userRegisterVm.UserName,
                FullName = userRegisterVm.FullName,
                Email = userRegisterVm.Email
            };

            var result = await _userManager.CreateAsync(user, userRegisterVm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "member");
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVM userLoginVm)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser user = await _userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or email");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, userLoginVm.Password, userLoginVm.RememberMe, true);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError("", "account is lockout...");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or email");
                return View();
            }

            return RedirectToAction("Index","Home");

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userProfileVM = new UserProfileVM
            {
                UserProfileUpdateVM = new UserProfileUpdateVM
                {
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email
                }
            };

            return View(userProfileVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(UserProfileUpdateVM userProfileUpdateVm)
        {
            UserProfileVM userProfileVM = new();
            userProfileVM.UserProfileUpdateVM = userProfileUpdateVm;
            if (!ModelState.IsValid)
                return View(userProfileUpdateVm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.FullName = userProfileUpdateVm.FullName;
            user.Email = userProfileUpdateVm.Email;
            user.UserName = userProfileUpdateVm.UserName;

            if(userProfileUpdateVm.CurrentPassword != null && userProfileUpdateVm.NewPassword != null)
            {
                var respomse = await _userManager.ChangePasswordAsync(user, userProfileUpdateVm.CurrentPassword, userProfileUpdateVm.NewPassword);
                if(!respomse.Succeeded)
                {
                    foreach (var error in respomse.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(userProfileVM);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userProfileVM);
            }
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVm)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
                return RedirectToAction("NotFound", "Error");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token}, Request.Scheme);

            // Send email

            return Json(url);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(PasswordResetVM passwordResetVm)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(passwordResetVm.Email);
            if (user == null)
                return RedirectToAction("NotFound", "Error");

            var result = await _userManager.ResetPasswordAsync(user, passwordResetVm.Token, passwordResetVm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return View();
        }



    }
}
