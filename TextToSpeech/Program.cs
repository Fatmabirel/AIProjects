using System.Speech.Synthesis;

class Program
{
    static void Main(string[] args)
    {
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        speechSynthesizer.Volume = 100; // max volume
        speechSynthesizer.Rate = 0; // normal speech speed

        Console.Write("Enter text to convert to speech (or leave empty to exit): ");
        string textToSpeak = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(textToSpeak))
            return;
        speechSynthesizer.Speak(textToSpeak);
        Console.ReadLine();
    }
}