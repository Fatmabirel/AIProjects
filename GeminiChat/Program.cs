using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        // Read API key from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var apiKey = config["GeminiApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HATA: appsettings.json içinde 'GeminiApiKey' bulunamadı.");
            Console.WriteLine("Lütfen API anahtarınızı appsettings.json dosyasına ekleyin.");
            Console.ResetColor();
            return;
        }

        // HttpClient and Endpoint setup
        using var httpClient = new HttpClient();
        var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        httpClient.DefaultRequestHeaders.Add("X-goog-api-key", apiKey);

        // Create a list to keep chat history
        var chatHistory = new List<(string Sender, string Message)>();

        // Welcome message
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Gemini Chatbot'a hoş geldiniz!");
        Console.WriteLine("Çıkmak için 'çık' veya 'exit' yazabilirsiniz.\n");
        Console.ResetColor();

        // Infinite chat loop
        while (true)
        {
            // Get user input
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Siz: ");
            Console.ResetColor();
            var prompt = Console.ReadLine();

            // Exit conditions
            if (string.IsNullOrWhiteSpace(prompt) || prompt.Trim().ToLower() == "çık" || prompt.Trim().ToLower() == "exit")
                break;

            // Add user message to chat history
            chatHistory.Add(("Siz", prompt));

            // Request body for Gemini API
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send request to Gemini API with PostAsync
                var response = await httpClient.PostAsync(endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response and extract the answer
                    var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                    var answer = result
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    // Add Gemini's answer to chat history
                    chatHistory.Add(("Gemini", answer));

                    // Print the entire chat history
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Gemini Chatbot'a hoş geldiniz!");
                    Console.WriteLine("Çıkmak için 'çık' veya 'exit' yazabilirsiniz.\n");
                    Console.ResetColor();

                    foreach (var (Sender, Message) in chatHistory)
                    {
                        if (Sender == "Siz")
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Siz: ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("Gemini: ");
                        }
                        Console.ResetColor();
                        Console.WriteLine(Message + "\n");
                    }
                }
                else
                {
                    // If the response is not successful, notify the user
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: {response.StatusCode}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                // Notify the user in case of an exception
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
