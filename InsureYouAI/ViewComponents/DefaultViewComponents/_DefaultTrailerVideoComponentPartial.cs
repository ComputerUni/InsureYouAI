using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DefaultViewComponents
{
    public class _DefaultTrailerVideoComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultTrailerVideoComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var value = _context.TrailerVideos.FirstOrDefault();
            return View(value);
        }
    }
}
