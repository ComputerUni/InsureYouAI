using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult CommentList()
        {
            return View();
        }
    }
}
