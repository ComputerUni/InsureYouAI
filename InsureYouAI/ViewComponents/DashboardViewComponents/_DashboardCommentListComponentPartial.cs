using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardCommentListComponentPartial(InsureContext _context) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var values = _context.Comments.Include(x => x.AppUser).OrderByDescending(x => x.CommentId).Take(7).ToList();
            return View(values);
        }
    }
}
