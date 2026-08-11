using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace InsureYouAI.Controllers
{
    public class MessageController : Controller
    {
        private readonly InsureContext _context;
        private readonly AIService _aiService;

        public MessageController(InsureContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public IActionResult MessageList(int page = 1, string? priority = null)
        {
            ViewBag.ControllerName = "Gelen Mesajlar";
            ViewBag.PageName = "İletişim Panelinden Gönderilen Mesaj Listesi";

            ViewBag.SelectedPriority = priority;

            var query = _context.Messages.AsQueryable();

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(x => x.Priority == priority);
            }

            var messages = query.ToList();
            return View(messages.ToPagedList(page, 8));
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {
            var combinedText = $"{message.Subject} - {message.MessageDetail}";
            var predictedCategory = await _aiService.PredictCategory(combinedText);
            var predictedPriority = await _aiService.PredictPriority(combinedText);

            Console.WriteLine("Category: " + predictedCategory);
            Console.WriteLine("Priority: " + predictedPriority);

            message.AICategory = predictedCategory;
            message.Priority = predictedPriority;
            message.IsRead = false;
            message.SendDate = DateTime.Now;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("MessageList");
        }

        [HttpGet]
        public IActionResult UpdateMessage(int id)
        {
            var value = _context.Messages.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }

        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }
    }
}
