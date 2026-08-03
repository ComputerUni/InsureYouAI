using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailNextAndPrevPostComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke(int id)
        {
            var article = _context.Articles.FirstOrDefault(x => x.ArticleId == id);

            var prevArticle = _context.Articles.Where(x => x.ArticleId < id).OrderByDescending(x => x.ArticleId).Select(x => x.Title).FirstOrDefault();
            var nextArticle = _context.Articles.Where(x => x.ArticleId > id).OrderBy(x => x.ArticleId).Select(x => x.Title).FirstOrDefault();

            ViewBag.prevArticleTitle = prevArticle;
            ViewBag.nextArticleTitle = nextArticle;

            return View(article);
        }
    }
}
