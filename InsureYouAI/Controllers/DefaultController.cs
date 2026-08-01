using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    [AllowAnonymous]
    public class DefaultController(InsureContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult SendMessage()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();

            #region AIAnaliz
            string apiKey = "";
            string prompt = $"Sen InsureYouAI Sigorta Şirketi'nin dijital müşteri hizmetleri asistanısın. \r\nGörevin, müşterilerden gelen mesajlara otomatik olarak yanıt vermek.\r\n\r\nGENEL DAVRANIŞ KURALLARI:\r\n- Samimi ama kurumsal bir dil kullan. Ne aşırı resmi ne de aşırı samimi ol.\r\n- Her zaman nazik, sabırlı ve anlayışlı ol.\r\n- Yanıtların net, kısa ve anlaşılır olsun. Gereksiz uzatma.\r\n- Müşteriyi her zaman \"Sayın Müşterimiz\" veya ismiyle hitap et.\r\n- Türkçe dilbilgisi kurallarına tam uy, yazım hatası yapma.\r\n\r\nYANIT VEREBİLECEĞİN KONULAR:\r\n- Kasko, trafik, sağlık, konut, seyahat ve hayat sigortası bilgilendirme\r\n- Poliçe yenileme, iptal ve değişiklik talepleri\r\n- Hasar bildirimi ve hasar süreç bilgisi\r\n- Prim ödeme ve ödeme planı soruları\r\n- Teklif ve fiyat bilgisi talepleri\r\n- Şikayet ve öneri bildirimleri\r\n- Genel sigorta merak ve soruları\r\n\r\nYANIT VERME KURALLARI:\r\n- Sigorta dışı konularda \"Bu konuda size yardımcı olamıyorum, sigorta ile ilgili sorularınız için buradayım.\" de.\r\n- Kesin fiyat teklifi verme, bunun yerine \"Uzmanlarımız en kısa sürede sizinle iletişime geçecektir.\" yönlendir.\r\n- Müşteri hasar bildirimi yapıyorsa aciliyet hissetir, empati kur ve süreci anlat.\r\n- Şikayet mesajlarında savunmaya geçme, önce empati kur sonra çözüm sun.\r\n- Yanıtın sonuna her zaman \"Başka bir konuda yardımcı olabilir miyim?\" ekle.\r\n\r\nYANIT FORMATI:\r\n- Selamlama ile başla\r\n- Ana yanıtı ver\r\n- Gerekiyorsa yönlendirme ekle\r\n- Kapanış cümlesiyle bitir\r\n\r\nÖRNEK SELAMLAMA:\r\n\"Merhaba, InsureYouAI Müşteri Hizmetleri'ne hoş geldiniz.\"\r\n\r\nÖRNEK KAPANIS:\r\n\"İyi günler dileriz. InsureYouAI ailesi olarak her zaman yanınızdayız.\"\r\n\r\n Kullanıcının sana gönderdiği mesaj şu şekilde: {message.MessageDetail}";

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://openrouter.ai/api/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                model = "openrouter/free",
                temperature = 0.5,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("v1/chat/completions", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);

            string textContent = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Mesajınız alınmıştır";

            var subjectRequestBody = new
            {
                model = "openrouter/free",
                temperature = 0.5,
                messages = new[]
               {
                     new { role = "user", content = $"Aşağıdaki müşteri mesajı için kısa ve profesyonel bir email konusu yaz. Sadece konu başlığını yaz, başka hiçbir şey yazma:\n\n{message.MessageDetail}" }
                }
            };


            var subjectContent = new StringContent(JsonSerializer.Serialize(subjectRequestBody), Encoding.UTF8, "application/json");
            var subjectResponse = await httpClient.PostAsync("v1/chat/completions", subjectContent);
            var subjectResponseString = await subjectResponse.Content.ReadAsStringAsync();

            using var subjectDoc = JsonDocument.Parse(subjectResponseString);
            string subjectTextContent = subjectDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "InsureYouAI Email Yanıtı";





            #endregion

            #region EmailGonderme
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("InsureYouAI Admin", "deneme@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", message.Email);
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = textContent;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = subjectTextContent;

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("deneme@gmail.com", "uygulama sifresi");
            client.Send(mimeMessage);
            client.Disconnect(true);
            #endregion

            return RedirectToAction("Index");
        }

        public PartialViewResult SubscribeEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult SubscribeEmail(string email)
        {
            return View();
        }
    }
}
