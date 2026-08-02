using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardSubCharts1ComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var policyCount = _context.Policies.GroupBy(x => x.PolicyType).Select(p => new { Type = p.Key, Count = p.Count() }).ToList();

            //Js için convert işlemi yaptık.
            ViewBag.policyData = JsonConvert.SerializeObject(policyCount);
            ViewBag.policyCounts = policyCount.Select(x => x.Count).ToList();
            return View();
        }
    }
}
