using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AppUserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult UserList()
        {
            var values = _userManager.Users.ToList();
            return View(values);
        }

        public async Task<IActionResult> UserProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            return View();
        }
    }
}
