using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailContentComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
