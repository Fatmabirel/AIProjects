using AssemblyAI;
using AssemblyAI.Transcripts;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SpeechToText;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();
        var apiKey = config["AssemblyAIApiKey"];
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        string audioFilePath = Path.Combine(exePath, "audio.mp3");

        Console.WriteLine($"Looking for audio file at: {audioFilePath}");

        if (!File.Exists(audioFilePath))
        {
            Console.WriteLine("Audio file not found! Please ensure 'audio.mp3' is in the same directory as the executable.");
            return;
        }

        try
        {
            Console.WriteLine("Initializing AssemblyAI client");
            var client = new AssemblyAIClient(apiKey);

            Console.WriteLine("Speech to text has been started");
            var transcript = await client.Transcripts.TranscribeAsync(
                new FileInfo(audioFilePath)
            );

            Console.WriteLine($"Translation has been completed: {transcript.Status}");

            if (transcript.Status == TranscriptStatus.Error)
            {
                Console.WriteLine("Error: " + transcript.Error);
                return;
            }

            Console.WriteLine("Text: ");
            if (string.IsNullOrEmpty(transcript.Text))
            {
                Console.WriteLine("Translation has been completed but could not convert to text!.");
            }
            else
            {
                Console.WriteLine(transcript.Text);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}