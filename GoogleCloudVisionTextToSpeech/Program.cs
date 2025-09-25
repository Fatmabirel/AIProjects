using System;
using System.IO;
using Google.Cloud.TextToSpeech.V1;
using NAudio.Wave;

class Program
{
    static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
            @"C:\Users\fatma\OneDrive\Masaüstü\practical-now-471417-u2-d81774d4f75f.json");

        var client = TextToSpeechClient.Create();

        var input = new SynthesisInput
        {
            Text = "Hallo, ich bin Fatma. Ich habe eine Bruder and fünf sister. Ich komme aus der Turkei, Ich wohne in Tekirdağ"
        };

        var voice = new VoiceSelectionParams
        {
            LanguageCode = "de-DE",
            SsmlGender = SsmlVoiceGender.Male
        };

        var config = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3
        };

        var response = client.SynthesizeSpeech(input, voice, config);

        using (var ms = new MemoryStream(response.AudioContent.ToByteArray()))
        using (var mp3Reader = new Mp3FileReader(ms))
        using (var waveOut = new WaveOutEvent())
        {
            waveOut.Init(mp3Reader);
            waveOut.Play();
            Console.WriteLine("Ses çalıyor, çıkmak için bir tuşa bas...");
            Console.ReadKey();
        }
    }
}
