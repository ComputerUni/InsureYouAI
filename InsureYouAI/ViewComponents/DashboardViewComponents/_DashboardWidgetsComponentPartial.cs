using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardWidgetsComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.v1 = _context.Articles.Count();
            ViewBag.v2 = _context.Categories.Count();
            ViewBag.v3 = _context.Comments.Count();
            ViewBag.v4 = _context.Users.Count();

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;
            var lastMonth = DateTime.Now.AddMonths(-1).Month;
            var lastMonthYear = DateTime.Now.AddMonths(-1).Year;

            var thisMonthArticles = _context.Articles.Count(x => x.CreatedDate.Month == thisMonth && x.CreatedDate.Year == thisYear);
            var lastMonthArticles = _context.Articles.Count(x => x.CreatedDate.Month == lastMonth && x.CreatedDate.Year == lastMonthYear);

            ViewBag.articleIncrease = lastMonthArticles == 0 && thisMonthArticles > 0 ? 100 :
    lastMonthArticles == 0 ? 0 :
    Math.Round((double)(thisMonthArticles - lastMonthArticles) / lastMonthArticles * 100, 1);


            var thisMonthComments = _context.Comments.Count(x => x.CommentDate.Month == thisMonth && x.CommentDate.Year == thisYear);
            var lastMonthComments = _context.Comments.Count(x => x.CommentDate.Month == lastMonth && x.CommentDate.Year == lastMonthYear);

            ViewBag.commentIncrease = lastMonthComments == 0 && thisMonthComments > 0 ? 100 :
    lastMonthComments == 0 ? 0 :
    Math.Round((double)(thisMonthComments - lastMonthComments) / lastMonthComments * 100, 1);


            return View();
        }
    }
}
