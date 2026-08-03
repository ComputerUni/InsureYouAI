using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardTopSellingComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var total = _context.Policies.Count();
            var policiesTypes = _context.Policies.GroupBy(x => x.PolicyType).Select(g => new { Type = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Take(5).ToList();

            ViewBag.types = policiesTypes;
            ViewBag.total = total;

            return View();
        }
    }
}
