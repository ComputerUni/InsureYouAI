using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ImageAIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly string apiKey = "";
        private static readonly string requestUrl = "https://router.huggingface.co/hf-inference/models/stabilityai/stable-diffusion-3-medium-diffusers";
        public ImageAIController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CreateImageWithAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageWithAI(string prompt)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                inputs = prompt,
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.error = "Hugging Face Hatası:" + await response.Content.ReadAsStringAsync();
                return View();
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(imageBytes);
            ViewBag.ImageBase64 = $"data:image/png;base64,{base64}";


            return View();
        }
    }
}
