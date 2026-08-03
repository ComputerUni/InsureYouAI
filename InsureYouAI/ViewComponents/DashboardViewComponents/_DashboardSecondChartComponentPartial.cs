using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardSecondChartComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var next30Days = DateTime.Now.AddDays(30);

            ViewBag.v1 = _context.Policies.Where(x => x.Status == "Active").Count();
            ViewBag.v2 = _context.Policies.Where(x => x.EndDate <= DateTime.Now).Count();
            ViewBag.v3 = _context.Policies.Where(x => x.EndDate >= DateTime.Now && x.EndDate <= next30Days).Count();
            ViewBag.v4 = _context.Policies.Count();

            return View();
        }
    }
}
