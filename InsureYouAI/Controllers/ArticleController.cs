using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace InsureYouAI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly InsureContext _context;

        public ArticleController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult ArticleList(int page = 1)
        {
            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "Makale Listesi";
            var articles = _context.Articles.Include(x => x.AppUser).ToList();
            return View(articles.ToPagedList(page, 8));
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "Yeni Makale Oluştur";
            var categories = _context.Categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString()
            }).ToList();

            ViewBag.categories = categories;

            var authors = _context.Users.Select(x => new SelectListItem
            {
                Text = x.Name + " " + x.Surname,
                Value = x.Id
            }).ToList();

            ViewBag.authors = authors;

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

            var categories = _context.Categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
                Selected = x.CategoryId == value.CategoryId
            }).ToList();

            ViewBag.categories = categories;

            var authors = _context.Users.Select(x => new SelectListItem
            {
                Text = x.Name + " " + x.Surname,
                Value = x.Id,
                Selected = x.Id == value.AppUserId
            }).ToList();

            ViewBag.authors = authors;


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
            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "AI ile Yeni Makale Oluştur";
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
