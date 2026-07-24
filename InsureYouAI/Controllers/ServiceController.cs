using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly InsureContext _context;

        public ServiceController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult ServiceList()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            var value = _context.Services.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        public IActionResult DeleteService(int id)
        {
            var value = _context.Services.Find(id);
            _context.Services.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        public async Task<IActionResult> CreateServiceWithAI()
        {
            var apiKey = "";
            var requestUrl = "https://openrouter.ai/api/v1/chat/completions";
            var prompt = "Bir sigorta şirketi için hizmetler bölümü hazırlamanı. Burada 5 farklı hizmet olmalı. Bana maksimum 100 karakterden oluşan cümlelerle 5 tane hizmet içeriği yazar mısın?";
            var requestBody = new
            {
                model = "openrouter/free",
                messages = new[]
                   {
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
            var serviceText = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            var services = serviceText.Split('\n')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                .ToList();
            ViewBag.services = services;

            return View();
        }
    }
}
