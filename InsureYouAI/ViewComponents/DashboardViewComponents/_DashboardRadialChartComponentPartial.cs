using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardRadialChartComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var startOfLastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var next30Days = DateTime.Now.AddDays(30);

            ViewBag.v1 = _context.Policies.Count();
            ViewBag.v2 = _context.Policies.Where(x => x.Status == "Active").Count();
            ViewBag.v3 = _context.Policies.Where(x => x.StartDate >= startOfLastMonth && x.StartDate < startOfMonth).Count();
            ViewBag.v4 = _context.Policies.Sum(x => x.PremiumAmount);
            ViewBag.v5 = _context.Policies.Where(x => x.EndDate >= DateTime.Now && x.EndDate <= next30Days).Count();

  
            return View();
        }
    }
}
