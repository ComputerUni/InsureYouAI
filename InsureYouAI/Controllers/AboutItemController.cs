using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutItemController : Controller
    {
        private readonly InsureContext _context;

        public AboutItemController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult AboutItemList()
        {
            var aboutItems = _context.AboutItems.ToList();
            return View(aboutItems);
        }

        [HttpGet]
        public IActionResult CreateAboutItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Add(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public IActionResult UpdateAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Update(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        public IActionResult DeleteAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            _context.AboutItems.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAboutItemWithAI()
        {
            var prompt = "Bir sigorta şirketi için 'Hakkımızda' sayfasında kullanılacak, güven verici ve etkileyici kısa madde metinleri oluştur. Örnek: 'Geleceğinizi güvence altına alan kapsamlı sigorta çözümleri sunuyoruz.' gibi. En az 10 farklı madde yaz. Her madde tek cümle olsun.";
            var apiKey = "";
            var requestUrl = "https://openrouter.ai/api/v1/chat/completions";
            var requestBody = new
            {
                model = "openrouter/free",
                messages = new[]
                   {
                        new
                        {
                            role = "system",
                            content = "Sen bir sigorta şirketi için çalışan içerik yazarı yapay zekasın. Kullanıcının isteğine göre, 'Hakkımızda' sayfasında kullanılacak kısa, etkileyici ve güven verici madde metinleri üret. Her maddeyi yeni satırda yaz, numara veya tire kullanma, sadece düz cümle olsun."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        },
                    },
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var response = await client.PostAsync(requestUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutText = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            ViewBag.aboutItem = aboutText;
            return View();
        }
    }
}
