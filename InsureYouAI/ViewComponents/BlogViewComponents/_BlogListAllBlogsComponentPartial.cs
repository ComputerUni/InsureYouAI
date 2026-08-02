using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace InsureYouAI.ViewComponents.BlogViewComponents
{
    public class _BlogListAllBlogsComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _BlogListAllBlogsComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

            int page = int.TryParse(HttpContext.Request.Query["page"], out int p) ? p : 1;

            var values = _context.Articles
                .Include(x => x.Category)
                .Include(y => y.AppUser)
                .Include(z => z.Comments)
                .ToPagedList(page, 6);

            return View(values);
        }
    }
}
