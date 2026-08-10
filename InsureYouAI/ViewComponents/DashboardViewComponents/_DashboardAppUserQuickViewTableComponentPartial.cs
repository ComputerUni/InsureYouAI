using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardAppUserQuickViewTableComponentPartial(InsureContext _context, UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _context.Users.GroupJoin(_context.Policies, user => user.Id, policy => policy.AppUserId, (user, policies) => new UserPolicySummaryViewModel
            {
                UserId = user.Id,
                ImageUrl = user.ImageUrl,
                UserName = user.UserName,
                FullName = user.Name + " " + user.Surname,
                PolicyCount = policies.Count(),
                TotalPremium = policies.Sum(p => (decimal?)p.PremiumAmount) ?? 0
            }).OrderByDescending(p => p.PolicyCount).ToListAsync();

            return View(values);
        }
    }
}
