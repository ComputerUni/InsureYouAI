using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace InsureYouAI.ViewComponents.BlogViewComponents
{
    public class _BlogListByCategoryComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke(int id)
        {
            int page = int.TryParse(HttpContext.Request.Query["page"], out int p) ? p : 1;
            var values = _context.Articles.Include(x => x.Category).Include(y => y.AppUser).Include(z => z.Comments).Where(y => y.CategoryId == id).ToPagedList(page, 5);
            return View(values);
        }
    }
}
