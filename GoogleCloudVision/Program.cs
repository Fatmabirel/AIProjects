using System;
using Google.Cloud.Vision.V1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Input image path:");
        Console.WriteLine();
        string impagePath = Console.ReadLine();

        string credentialPath = @"C:\Users\fatma\OneDrive\Masaüstü\practical-now-471417-u2-cc974e129720.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        try
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(impagePath);
            var response = client.DetectText(image);
            Console.WriteLine("Text in the image:");
            Console.WriteLine();
            foreach (var annotation in response)
            {
                if (!string.IsNullOrEmpty(annotation.Description))
                {
                    Console.WriteLine($"{annotation.Description}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occured: " + ex.ToString());
        }
    }
}
