using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Data;
using E_Diary.WEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_Diary.WEB.Helpers;

namespace E_Diary.WEB.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private ASPIdentityDBContext _context;
        public AccountController(ASPIdentityDBContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.FirstOrDefault(x => x.UserName == model.Username);
                if (user != null)
                {
                    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (signInResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                        if (await _userManager.IsInRoleAsync(user,"admin"))
                            return RedirectToAction("Index", "Admin");
                        if (await _userManager.IsInRoleAsync(user,"teacher"))
                            return RedirectToAction("Index", "Teacher");
                        if (await _userManager.IsInRoleAsync(user,"schoolboy"))
                            return RedirectToAction("Index", "Schoolboy");
                    }
                }
                ViewBag.Message = "Пользователь с указанными данными не найден. Проверьте имя пользователя и пароль";
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            ProfileMainModel model = new ProfileMainModel();
            User? user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.Name = user.Name;
                model.Surname = user.Surname;
                model.Patronymic = user.Patronymic;
                model.Gender = user.Gender;
                model.Email = user.Email;
            }

            return View(model);
        }
        [Authorize]
        [HttpPost]
        IActionResult Profile(ProfileMainModel profileModel)
        {
            return View(profileModel.Email);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult GetGeneratedPassword()
        {
            return Json(PasswordGeneratorHelper.GenerateRandomPassword());
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetResetPasswordLink(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                return View((email, resetPasswordToken));
            }
            return NotFound();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ResetPasswordViewModel vm = new() { Email = email, Token = token };
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            User? user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
