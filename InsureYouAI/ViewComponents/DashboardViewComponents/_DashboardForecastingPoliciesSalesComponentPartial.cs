using InsureYouAI.Context;
using InsureYouAI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardForecastingPoliciesSalesComponentPartial(InsureContext _context, ForecastService _forecastService) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var salesData = _context.Policies.Where(p => p.StartDate < new DateTime(2026, 8, 1))
                .AsEnumerable()
                .GroupBy(p => new { p.StartDate.Year, p.StartDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new PolicySalesData
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    SaleCount = g.Count()
                })
                .ToList();

            var forecast = _forecastService.GetForecast(salesData, horizon: 5);

            var months = new[] { "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

            var lastActualValue = (double)salesData.Last().SaleCount;

            var forecastList = Enumerable.Range(0, 5).Select(i => new
            {
                Month = months[i],
                Value = (int)Math.Round(forecast.ForecastedValues[i]),
                Lower = (int)Math.Round(forecast.LowerBoundValues[i]),
                Upper = (int)Math.Round(forecast.UpperBoundValues[i])
            }).ToList();

            var result = new List<object>();

            for (int i = 0; i < forecastList.Count; i++)
            {
                var item = forecastList[i];
                var previousValue = i == 0 ? lastActualValue : forecastList[i - 1].Value;
                var change = previousValue == 0 ? 0 : ((item.Value - previousValue) / previousValue * 100);

                result.Add(new
                {
                    item.Month,
                    item.Value,
                    item.Lower,
                    item.Upper,
                    ChangePercent = Math.Round(change, 1),
                    IsPositive = change >= 0
                });
            }

            ViewBag.Forecast = result;


            return View();
        }
    }
}

