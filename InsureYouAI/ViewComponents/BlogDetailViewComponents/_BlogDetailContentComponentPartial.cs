using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailContentComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogDetailContentComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            var values = _context.Articles.Include(x => x.AppUser).Include(x => x.Category).Include(x => x.Comments).Where(x => x.ArticleId == id).FirstOrDefault();
            return View(values);
        }
    }
}
