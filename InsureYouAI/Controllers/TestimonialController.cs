using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly InsureContext _context;

        public TestimonialController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult TestimonialList()
        {
            var sliders = _context.Testimonials.ToList();
            return View(sliders);
        }

        [HttpGet]
        public IActionResult CreateTestimonial()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Add(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        [HttpGet]
        public IActionResult UpdateTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Update(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        public IActionResult DeleteTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            _context.Testimonials.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        public async Task<IActionResult> CreateTestimonialWithAI()
        {
            var apiKey = "";
            var requestUrl = "https://openrouter.ai/api/v1/chat/completions";
            var prompt = "Bir sigorta şirketi için müşteri deneyimlerine dair yorum oluşturmak istiyorum. Bu alanda Türkçe olarak 6 tane yorum, 6 tane müşteri adı ve soyadı, bu müşterilerin unvanı olsun. Buna göre içeriği hazırla.";
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
            var testimonialText = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            var testimonials = testimonialText.Split('\n')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                .ToList();
            ViewBag.testimonials = testimonials;

            return View();
        }
    }
}
