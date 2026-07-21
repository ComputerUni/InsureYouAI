using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureYouAI.Controllers
{
    public class AboutController : Controller
    {
        private readonly InsureContext _context;

        public AboutController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult AboutList()
        {
            var abouts = _context.Abouts.ToList();
            return View(abouts);
        }

        [HttpGet]
        public IActionResult CreateAbout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAbout(About about)
        {
            _context.Abouts.Add(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public IActionResult UpdateAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAbout(About about)
        {
            _context.Abouts.Update(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        public IActionResult DeleteAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            _context.Abouts.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAboutWithAI()
        {
            var prompt = "Bir sigorta şirketi için 'Hakkımızda' sayfası metni yaz. Şirket; bireysel ve kurumsal müşterilere hayat sigortası, sağlık sigortası, araç sigortası, konut sigortası ve işyeri sigortası alanlarında kapsamlı çözümler sunmaktadır. Yılların deneyimiyle sektörde güvenilir bir konuma ulaşmış olan şirket, müşteri memnuniyetini her zaman ön planda tutmaktadır. Hasarlarda hızlı çözüm, şeffaf iletişim ve uzman kadrosuyla müşterilerine kesintisiz destek sağlamaktadır. Metnin tonu kurumsal ve güven verici olsun, geleceğe dair vizyon ve misyon vurgusu da yapılsın.";
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
                            content = "Sen bir sigorta şirketi için çalışan, içerik yazarlığı yapan bir yapay zekasın. Kullanıcıların verdiği özet ve anahtar kelimelere göre, sigortacılık sektörüyle ilgili makale üret. En az 1000 karakter olsun. Makaleyi düz yazı formatında yaz, kesinlikle markdown kullanma, yıldız (*), tire (-), diyez (#) gibi semboller kullanma. Paragraflar halinde akıcı bir Türkçe ile yaz."
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
            ViewBag.about = aboutText;
            return View();
        }

    }
}
