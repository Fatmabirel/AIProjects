using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main()
    {
        var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();
        var apiKey = config["GoogleCloudVisionApiKey"];

        var httpClient = new HttpClient();

        while (true)
        {
            Console.Write("Çevirmek istediğin metni gir (çıkmak için boş bırak): ");
            string textToTranslate = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(textToTranslate))
                break;

            Console.Write("Hedef dili gir (örn: en, de, fr, ru): ");
            string targetLang = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetLang))
            {
                Console.WriteLine("Hedef dil boş olamaz!");
                continue;
            }

            var requestData = new
            {
                q = textToTranslate,
                source = "tr",   // kaynak dili Türkçe sabit
                target = targetLang,
                format = "text"
            };

            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(requestData),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await httpClient.PostAsync(
                    $"https://translation.googleapis.com/language/translate/v2?key={apiKey}",
                    content
                );

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var translated = JObject.Parse(jsonResponse)["data"]["translations"][0]["translatedText"].ToString();

                Console.WriteLine($"Çeviri ({targetLang}): {translated}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        Console.WriteLine("Program kapatıldı.");
    }
}
