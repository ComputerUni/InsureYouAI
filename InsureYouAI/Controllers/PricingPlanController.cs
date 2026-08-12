using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class PricingPlanController : Controller
    {
        private readonly InsureContext _context;

        public PricingPlanController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult PricingPlanList()
        {
            ViewBag.ControllerName = "Sigorta Planları";
            ViewBag.PageName = "Sigorta Planları Listesi";
            var pricingPlans = _context.PricingPlans.ToList();
            return View(pricingPlans);
        }

        [HttpGet]
        public IActionResult CreatePricingPlan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Add(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        [HttpGet]
        public IActionResult UpdatePricingPlan(int id)
        {
            ViewBag.ControllerName = "Sigorta Planları";
            ViewBag.PageName = "Sigorta Planı Güncelleme";
            var value = _context.PricingPlans.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Update(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        public IActionResult DeletePricingPlan(int id)
        {
            var value = _context.PricingPlans.Find(id);
            _context.PricingPlans.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        public IActionResult ChangeStatus(int id)
        {
            var value = _context.PricingPlans.Find(id);
            if(value.IsFeature == true)
            {
                value.IsFeature = false;
            }
            else
            {
                value.IsFeature = true;
            }

            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }


        [HttpGet]
        public IActionResult CreaterUserCustomizePlan()
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Kullanıcıya Özel AI Destekli Sigorta Planı Belirleme";
            var model = new AIInsuranceRecommendationViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreaterUserCustomizePlan(AIInsuranceRecommendationViewModel model)
        {
            string apiKey = "";
            string requestUrl = "https://openrouter.ai/api/v1/chat/completions";

            var userJson = JsonConvert.SerializeObject(model);
            var prompt = $@"
Sen profesyonel bir sigorta uzmanı AI asistanısın.
Aşağıdaki kullanıcının bilgilerini dikkatlice analiz ederek en uygun sigorta paketini öner.

Paketler:
1) Premium Paket (599 TL/ay): Yatarak tedavi, check-up, geniş yol yardım, yurtiçi seyahat güvencesi.
2) Standart Paket (449 TL/ay): Acil sağlık, müşteri hizmetleri, kaza sonrası tıbbi destek.
3) Ekonomik Paket (339 TL/ay): Temel sağlık, temel yol yardım.

Kullanıcı bilgileri:
{userJson}

TÜRKİYE 2026 EKONOMİK BAĞLAMI:
- Asgari ücret ~29.000 TL, ortalama maaş 40.000-60.000 TL arasındadır.
- İstanbul, Ankara, İzmir gibi büyük şehirlerde kira 15.000-25.000 TL, temel yaşam giderleri aylık 10.000-15.000 TL civarındadır.
- Bu koşullarda bireylerin sigorta için ayırabildiği bütçe oldukça kısıtlıdır, bunu her zaman göz önünde bulundur.

ÖNEMLİ KARAR KRİTERLERİ:
- Aylık bütçe 350 TL altındaysa veya kullanıcı asgari ücretli görünüyorsa Ekonomik paketi öner.
- Aylık bütçe 350-500 TL arasındaysa Standart paketi öner.
- Aylık bütçe 500 TL üstündeyse ve geliri buna uygunsa Premium paketi değerlendir.
- Kullanıcının belirttiği bütçe paket fiyatını karşılamıyorsa kesinlikle bir alt pakete yönlendir, zorlama.
- Büyük şehirde (İstanbul, Ankara, İzmir) yaşıyorsa yaşam maliyeti yüksek olduğundan bütçeyi daha kısıtlı değerlendir.
- Kronik hastalık varsa ve bütçe elveriyorsa bir üst pakete yönlendir, bütçe elvermiyorsa Ekonomik paketi sağlık odaklı öner.
- Sık seyahat ediyorsa (Yılda 3-5 veya Sık seyahat) ve bütçe uygunsa yurtiçi seyahat güvencesi olan Premium paketi önceliklendir.
- Çocuk sayısı 2 ve üzeriyse aile giderleri yüksek olacağından bütçeye uygun en geniş kapsamlı paketi öner.
- Yaş 50 üzerindeyse sağlık riskleri arttığından sağlık kapsamı geniş paketi tercih et.
- Meslek fiziksel risk içeriyorsa (inşaat, madencilik, güvenlik, fabrika işçisi vb.) kaza teminatı olan üst paketi öner.
- onerilenPaket ile ikinciSecenek ASLA aynı olamaz.
- Gerçekçi ve ulaşılabilir öneriler sun, kullanıcıyı karşılayamayacağı paketlere yönlendirme.

KURALLAR:
- Yanıtın yalnızca aşağıdaki JSON formatında olsun.
- Kesinlikle ```json veya ``` kullanma.
- JSON dışında hiçbir açıklama, başlık veya metin yazma.
- ""neden"" alanı Türkçe, 6-7 cümle, akıcı ve anlaşılır olsun.
- ""onerilenPaket"" ve ""ikinciSecenek"" alanları yalnızca şu değerlerden biri olsun: Premium, Standart, Ekonomik.

{{
  ""onerilenPaket"": ""Premium | Standart | Ekonomik"",
  ""ikinciSecenek"": ""Premium | Standart | Ekonomik"",
  ""neden"": ""Kullanıcının profiline ve Türkiye 2026 ekonomik koşullarına göre analiz metni""
}}";

            var requestBody = new
            {
                model = "openrouter/free",
                messages = new[]
                {
            new { role = "system", content = "Sen bir sigorta uzmanı AI asistanısın. Kullanıcı bilgilerine göre en uygun sigorta paketini JSON formatında öner." },
            new { role = "user",   content = prompt }
        },
                temperature = 0.7
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.PostAsync(requestUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutText = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            try
            {
                var cleaned = aboutText.Replace("```json", "").Replace("```", "").Trim();
                var result = JsonConvert.DeserializeObject<dynamic>(cleaned);

                model.RecommendedPackage = result?.onerilenPaket;
                model.SecondBestPackage = result?.ikinciSecenek;
                model.AnalysisText = result?.neden;
            }
            catch
            {
                model.AnalysisText = aboutText;
            }

            return View(model);
        }


    }
}

