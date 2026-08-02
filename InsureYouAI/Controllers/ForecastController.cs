using InsureYouAI.Context;
using InsureYouAI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class ForecastController(InsureContext _context, ForecastService _forecastService) : Controller
    {
        public IActionResult Index()
        {
            var salesData = _context.Policies.GroupBy(p => new { p.StartDate.Year, p.StartDate.Month }).AsEnumerable().Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() }).AsEnumerable().Select(g => new PolicySalesData { Date = new DateTime(g.Year, g.Month, 1), SaleCount = g.Count })
                .OrderBy(x => x.Date).ToList();

            var forecast = _forecastService.GetForecast(salesData, horizon: 3);

            ViewBag.forecast = forecast;

            return View(salesData);
        }
    }
}
