using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardSubCharts3ComponentPartial(InsureContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var expenseData = await _context.Expenses.Where(e => e.ProcessDate >= startOfMonth).GroupBy(e => e.Detail).Select(g => new { Category = g.Key, TotalAmount = g.Sum(x => x.Amount) }).ToListAsync();

            ViewBag.ExpenseLabels = expenseData.Select(x => x.Category).ToList();
            ViewBag.ExpenseValues = expenseData.Select(x => x.TotalAmount).ToList();

            return View();
        }
    }
}
