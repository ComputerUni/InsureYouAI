using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult BlogList()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public PartialViewResult GetBlog()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult GetBlog(string keyword)
        {
            return View();
        }
    }
}
