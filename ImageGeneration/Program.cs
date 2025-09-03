using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();
        var apiKey = config["HuggingFaceAIApiKey"];
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        Console.Write("🎨 Please enter a sentence to create an image (prompt): ");

        var userPrompt = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            Console.WriteLine("❌ Lütfen geçerli bir prompt girin.");
            return;
        }

        var requestData = new
        {
            prompt = userPrompt,
            response_format = "base64",
            model = "black-forest-labs/FLUX.1-schnell"
        };

        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://router.huggingface.co/together/v1/images/generations", content);

        if (response.IsSuccessStatusCode)
        {

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var base64 = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("b64_json")
                .GetString();

            var imageBytes = Convert.FromBase64String(base64);
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;

            var folderPath = Path.Combine(projectRoot, "Images");
            Directory.CreateDirectory(folderPath);
            var fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"✅ Image successfully created and saved in: {filePath}");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("❌ Error: " + error);
        }
    }
}