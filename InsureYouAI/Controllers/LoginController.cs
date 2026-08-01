using InsureYouAI.Dtos;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    [AllowAnonymous]
    public class LoginController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if(user == null)
            {
                ModelState.AddModelError("", "Sistemde bu bilgilere ait kullanıcı bulunamadı");
                return View(loginDto);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                return View(loginDto);
            }

            return RedirectToAction("UserList", "AppUser");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }
    }
}
