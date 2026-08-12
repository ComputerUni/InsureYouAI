using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace InsureYouAI.Controllers
{
    public class PolicyAnalysisWithAIController : Controller
    {
        private readonly string apiKey = "";
        private readonly string requestUrl = "https://openrouter.ai/api/v1/chat/completions";

        [HttpGet]
        public IActionResult PdfAnalyze()
        {
            ViewBag.ControllerName = "AI";
            ViewBag.PageName = "AI ile PDF Analizi";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PdfAnalyze(IFormFile pdfFile)
        {
            ViewBag.ControllerName = "AI";
            ViewBag.PageName = "AI ile PDF Analizi";

            if (pdfFile == null || pdfFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir PDF poliçe dosyası yükleyiniz.";
                return View();
            }

            string extractedText = await ExtractTextFromPdf(pdfFile);

            if(string.IsNullOrWhiteSpace(extractedText))
            {
                ViewBag.Error = "PDF içerisinden metin çıkarılamadı";
                return View();
            }

            string analysis = await AnalyzePolicyWithAI(extractedText);

            ViewBag.OriginalText = extractedText;
            ViewBag.AnalysisResult = analysis;

            return View();
        }

        private async Task<string> ExtractTextFromPdf(IFormFile pdfFile)
        {
            using var ms = new MemoryStream();
            await pdfFile.CopyToAsync(ms);
            ms.Position = 0;

            var sb = new StringBuilder();

            using (var document = PdfDocument.Open(ms))
            {
                foreach (var page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                    sb.AppendLine("\n");
                }
            }

            return sb.ToString();
        }


        private async Task<string> AnalyzePolicyWithAI(string policyText)
        {
            var prompt = $@"Aşağıdaki metin bir sigorta poliçesine aittir.
Görevlerin:
1) Poliçeyi 10 maddede özetle.
2) Neleri kapsar? (Madde madde yaz)
3) Neleri kapsamaz? (Madde madde yaz)
4) Müşteri için kritik uyarıları kalın yap .
5) Yanıtı düz yazı formatında üret. 

POLİÇE METNİ
{policyText}
";

            var requestBody = new
            {
                model = "openrouter/free",
                temperature = 0.2,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new {type = "text", text = prompt}
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var response = await client.PostAsync(requestUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var pdfText = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return pdfText;
        }
    }
}
