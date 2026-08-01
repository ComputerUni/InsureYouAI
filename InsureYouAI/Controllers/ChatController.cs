using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult ChatWithAI()
        {
            return View();
        }
    }
}
