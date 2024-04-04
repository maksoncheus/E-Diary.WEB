using E_Diary.WEB.Data.Entities;
using E_Diary.WEB.Data;
using E_Diary.WEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_Diary.WEB.Helpers;
using Microsoft.EntityFrameworkCore;
using TransportationService.WEB.Services;

namespace E_Diary.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ASPIdentityDBContext _context;
        private readonly MailService _mailService;
        public AccountController(ASPIdentityDBContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            MailService mailService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _mailService = mailService;
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
        public IActionResult GetResetPasswordLink()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetResetPasswordLink(string? authString)
        {
            if (authString == null) return Forbid();
            User? user = await _userManager.FindByEmailAsync(authString);
            if (user == null)
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == authString);
            if (user == null)
                return NotFound();
            string link = await GenerateResetPasswordLink(user);
            _mailService.SendResetLink(link, user.Email);
            string message = "На вашу почту было отправлено письмо с ссылкой для сброса пароля.";
            return RedirectToAction("Message", "Home", new { msg = message });
        }
        private async Task<string> GenerateResetPasswordLink(User user)
        {
            string resetToken = await _userManager
                 .GeneratePasswordResetTokenAsync(user);

            string? confirmationLink = Url.Action("ResetPassword",
              "Account", new
              {
                  userid = user.Id,
                  token = resetToken
              },
              protocol: HttpContext.Request.Scheme);
            if (confirmationLink == null) throw new ArgumentNullException(nameof(confirmationLink));
            return confirmationLink;
        }
        [HttpGet]
        public async Task<IActionResult> FindSuggestedUser(string? authString)
        {
            if (authString == null)
                return BadRequest("Укажите почту!");
            User? user = await _userManager.FindByEmailAsync(authString);
            if (user == null)
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == authString);
            if (user == null)
                return BadRequest("Такого пользователя не найдено");
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                ResetPasswordViewModel vm = new() { Id = userId, Token = token };
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            User? user = await _userManager.FindByIdAsync(vm.Id);
            if (user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
                if (result.Succeeded)
                {
                    string message = "Вы успешно изменили пароль своей учетной записи." +
                        " Теперь вы можете авторизоваться, используя новые данные";
                    return RedirectToAction("Message", "Home", new { msg = message });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
