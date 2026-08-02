using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutNavbarComponentPartial(UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            ViewBag.fullName = user?.Name + " " + user?.Surname;
            ViewBag.title = user?.Title;
            ViewBag.imageUrl = user?.ImageUrl;
            ViewBag.email = user?.Email;
            return View();
        }
    }
}
