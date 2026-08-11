using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static InsureYouAI.Controllers.ArticleController;

namespace InsureYouAI.Services
{
    public class AIService
    {
        private readonly string apiKey = "";
        private readonly string model = "openrouter/free";

        public async Task<string> PredictCategory(string messageText)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                new
                {
                    role = "system",
                    content = $"Aşağıdaki kullanıcı mesajını sigortacılık alanında kategorize et. Sadece kategori adı döndür. \n\n Mesaj: {messageText}\n\nOlası kategoriler:\n- Kasko\n- Trafik Sigortası\n- Sağlık Sigortası\n- Konut Sigortası\n- Hasar Bildirimi\n- Fiyat Teklifi\n- Poliçe Yenileme\n- Genel Soru\n- İletişim Talebi\n"
                },
            },
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(responseJson);
                var predictedCategory = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return predictedCategory.Trim();
            }
        }

        public async Task<string> PredictPriority(string messageText)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                new
                {
                    role = "system",
                    content = $@"Aşağıdaki kullanıcı mesajının aciliyet seviyesini belirle.
Sadece 3 seçenekten birini döndür: High, Medium, Low.

Kurallar: 
- Kaza, hasar, ödeme sorunları, acil durumlar -> High
- Fiyat teklifi, yenileme, teminat sorunları -> Medium
- Genel sorular, merak edilen bilgiler -> Low

Mesaj:
{messageText}
"
                },
            },
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(responseJson);
                var predictedPriority = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                Console.WriteLine("Priority response: " + predictedPriority);
                return predictedPriority.Trim();
            }
        }
    }
}


