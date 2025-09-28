using System;
using Google.Cloud.Language.V1;

class Program
{
    static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
           @"C:\Users\fatma\OneDrive\Masaüstü\practical-now-471417-u2-d81774d4f75f.json");
        // Google Cloud Natural Language API client oluştur
        var client = LanguageServiceClient.Create();

        Console.WriteLine("Duygu Analizi için bir cümle yazın:");
        string text = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("Boş metin girdiniz, lütfen tekrar deneyin.");
            return;
        }

        // Analiz için Document hazırla
        var document = new Document
        {
            Content = text,
            Type = Document.Types.Type.PlainText
        };

        // API çağrısı: Sentiment analizi
        var response = client.AnalyzeSentiment(document);

        // Genel metin analizi sonucu
        Console.WriteLine("\n--- Genel Analiz ---");
        Console.WriteLine($"Duygu Skoru: {response.DocumentSentiment.Score}");
        Console.WriteLine($"Yoğunluk (Magnitude): {response.DocumentSentiment.Magnitude}");

        // Cümle bazlı analiz
        Console.WriteLine("\n--- Cümle Bazlı Analiz ---");
        foreach (var sentence in response.Sentences)
        {
            Console.WriteLine($"Cümle: {sentence.Text.Content}");
            Console.WriteLine($"Skor: {sentence.Sentiment.Score}, Yoğunluk: {sentence.Sentiment.Magnitude}");
            Console.WriteLine();
        }
    }
}
