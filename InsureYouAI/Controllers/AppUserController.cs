using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly InsureContext _context;

        public AppUserController(UserManager<AppUser> userManager, InsureContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult UserList()
        {
            var values = _userManager.Users.ToList();
            return View(values);
        }

        public async Task<IActionResult> UserProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.title = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;

            var articleCount = await _context.Articles.Where(x => x.AppUserId == id).CountAsync();
            var commentCount = await _context.Comments.Where(x => x.AppUserId == id).CountAsync();

            ViewBag.articleCount = articleCount;
            ViewBag.commentCount = commentCount;

            //Kullanıcı Bilgilerini Çekme
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            //Kullanıcıya Ait Makale Listesi
            var articles = await _context.Articles.Where(x => x.AppUserId == id).Select(y => y.Content).ToListAsync();

            if (articles.Count == 0)
            {
                ViewBag.AIResult = "Bu kullancıya ait analiz yapılacak makale bulunamadı!";
                return View(user);
            }

            //Makaleleri tek bir metinde toplayalım.
            var allArticles = string.Join("\n\n", articles);

            var apiKey = "";

            //Promptun Yazılması
            var prompt = $@"
Sen bir sigorta sektöründe uzman bir içerik analistisin.
Elinizde, bir sigorta şirketinin çalışanının yazdığı tüm makaleler var. Bu makaleler
üzrenden çalışanın içerik üretim tarzını analiz et.

Analiz Başlıkları:

1) Konu çeşitliliği ve odak alanları (sağlık, hayat, kasko, tamamlayıcı, BES vb.)
2) Hedef kitle tahmini (bireysel/kurumsal, segment, persona)
3) Dil ve Anlatım Tarzı (tekniklik seviyesi, okunabilirlik, ikna gücü)
4) Sigorta terimlerini kullanma ve doğruluk düzeyi
5) Müşteri ihtiyaçlarına ve risk yönetimine odaklanma
6) Pazarlama/satış vurgusu, CTA netliği
7) Geliştirilmesi gereken alanlar ve net aksiyon maddeleri

Makaleler:

{allArticles}

Lütfen çıktıyı profesyonel rapor formatında, madde madde ve en sonda 5 maddelik aksiyon listesi ile ver.";

            //AI Kısmı

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "openrouter/free",
                    messages = new object[]
                    {
                        new { role = "system", content = "Sen sigorta şirketinde içerik analizi yapan bir uzmansın." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                };

                //Json Dönüşümleri
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.AIResult = "Open AI Hatası: " + response.StatusCode;
                    return View(user);
                }

                //Json Veri İçinden Yapıyı Okuma

                try
                {
                    using var doc = JsonDocument.Parse(responseText);
                    var aiText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                    ViewBag.AIResult = aiText ?? "Boş Yanıt Döndü.";
                }
                catch
                {
                    ViewBag.AIResult = "OpenAI yanıtı beklenen formatta değil";
                }

            }


            return View(user);
        }

        public async Task<IActionResult> UserCommentsProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.title = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;

            var articleCount = await _context.Articles.Where(x => x.AppUserId == id).CountAsync();
            var commentCount = await _context.Comments.Where(x => x.AppUserId == id).CountAsync();

            ViewBag.articleCount = articleCount;
            ViewBag.commentCount = commentCount;

            //Kullanıcı Bilgilerini Çekme
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            //Kullanıcıya Ait Yorum Listesi
            var comments = await _context.Comments.Where(x => x.AppUserId == id).Select(y => y.CommentDetail).ToListAsync();

            if (comments.Count == 0)
            {
                ViewBag.AIResult = "Bu kullancıya ait analiz yapılacak yorum bulunamadı!";
                return View(user);
            }

            //Yorumları tek bir metinde toplayalım.
            var allComments = string.Join("\n\n", comments);

            var apiKey = "";

            //Promptun Yazılması
            var prompt = $@"
Sen kullanıcı davranış analizi yapan bir yapay zeka uzmanısın.
Aşağıdaki yorumlara göre kullanıcı değerlendir.

Analiz Başlıkları:

1) Genel Duygu Durumu (pozitif/negatif/nötr)
2) Toksik içerik var mı? (örnekleriyle)
3) İlgi alanları / konu başlıkları
4) İletişim tarzı (samimi, resmi, agresif vb.)
5) Geliştirilmesi gereken iletişim alanları
6) 5 maddelik kısa özet.

Yorumlar:

{allComments}";

            //AI Kısmı

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "openrouter/free",
                    messages = new object[]
                    {
                        new { role = "system", content = "Sen kullanıcı yorum analizi yapan bir uzmansın." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                };

                //Json Dönüşümleri
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.AIResult = "Open AI Hatası: " + response.StatusCode;
                    return View(user);
                }

                //Json Veri İçinden Yapıyı Okuma

                try
                {
                    using var doc = JsonDocument.Parse(responseText);
                    var aiText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                    ViewBag.AIResult = aiText ?? "Boş Yanıt Döndü.";
                }
                catch
                {
                    ViewBag.AIResult = "OpenAI yanıtı beklenen formatta değil";
                }

            }


            return View(user);
        }
    }
}
