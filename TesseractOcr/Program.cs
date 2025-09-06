using Tesseract;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Image Path:");
        string imagePath = Console.ReadLine() ?? "";
        Console.ReadLine();

        string tessdataPath = @"C:\tessdata"; // Path to the tessdata directory
        try
        {
            // Initialize Tesseract OCR engine
            using var ocrEngine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);

            // Load the image
            using var img = Pix.LoadFromFile(imagePath);

            // Perform OCR on the image
            using var page = ocrEngine.Process(img);

            // Get the recognized text
            string recognizedText = page.GetText();

            // Write the recognized text to the output file        
            Console.WriteLine("OCR completed successfully. Output written to:");
            Console.WriteLine(recognizedText);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        Console.ReadLine();
    }
}
