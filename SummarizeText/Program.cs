using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // appsettings.json'dan API key al

        var apiKey = "api_key";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

        Console.WriteLine("Özetlenecek metni giriniz:");
        string inputText = Console.ReadLine();

        string[] modes = { "short", "medium", "long" };

        foreach (var mode in modes)
        {
            string prompt = mode switch
            {
                "short" => $"Give a very short summary of: {inputText}",
                "medium" => $"Give a medium-length summary of: {inputText}",
                "long" => $"Give a detailed summary of: {inputText}",
                _ => inputText
            };

            var requestData = new
            {
                inputs = prompt,
                options = new { wait_for_model = true }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api-inference.huggingface.co/models/sshleifer/distilbart-cnn-12-6", content);
            var result = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(result);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var summary = doc.RootElement[0].GetProperty("summary_text").GetString();
                    Console.WriteLine($"\n=== {mode.ToUpper()} SUMMARY ===");
                    Console.WriteLine(summary);
                }
                else if (doc.RootElement.TryGetProperty("error", out var error))
                {
                    Console.WriteLine($"\n=== {mode.ToUpper()} SUMMARY ===");
                    Console.WriteLine("Hata: " + error.GetString());
                }
                else
                {
                    Console.WriteLine($"\n=== {mode.ToUpper()} SUMMARY ===");
                    Console.WriteLine("Beklenmeyen response:\n" + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== {mode.ToUpper()} SUMMARY ===");
                Console.WriteLine("JSON parse hatası: " + ex.Message);
                Console.WriteLine("Ham response:\n" + result);
            }
        }

        Console.WriteLine("\nTamamlandı! Çıkmak için tuşa basın...");
        Console.ReadKey();
    }
}
