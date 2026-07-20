using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InsureYouAI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly InsureContext _context;

        public ArticleController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult ArticleList()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle(Article article)
        {
            article.CreatedDate = DateTime.Now;
            _context.Articles.Add(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult UpdateArticle(int id)
        {
            var value = _context.Articles.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateArticle(Article article)
        {
            _context.Articles.Update(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        public IActionResult DeleteArticle(int id)
        {
            var value = _context.Articles.Find(id);
            _context.Articles.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult CreateArticleWithAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticleWithAI(string prompt)
        {
            string apiKey = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

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

                var response = await client.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AIResponse>();
                    var content = result.choices[0].message.content;
                    ViewBag.article = content;
                }
                else
                {
                    ViewBag.article = "Bir hata oluştu" + response.StatusCode;
                }
                return View();
            }
        }

        public class AIResponse()
        {
            public List<Choice> choices { get; set; }
        }

        public class Choice()
        {
            public Message message { get; set; }
        }

        public class Message()
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
