using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DefaultViewComponents
{
    public class _DefaultAboutItemComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;

        public _DefaultAboutItemComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var aboutItems = _context.AboutItems.ToList();
            return View(aboutItems);
        }
    }
}
