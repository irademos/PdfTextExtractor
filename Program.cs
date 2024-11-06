using System;
using System;
using System.IO;
using System.Drawing;
using Tesseract;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
// using iText.Kernel.Pdf.Canvas.Renderer;
using iText.Kernel.Geom; // Use Geom for geometry related types
// using iText.Kernel.Geometry;
using System.Text.RegularExpressions;
using iText.Forms;
using System.Collections.Generic;
using iText.Forms.Fields;

       
class Program
{
    static void Main(string[] args)
    {      
        string folderPath = @"C:\Users\DemosChristopher\PdfTextExtractor\Test PDFs";
        string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");

        string text = "";
        foreach (string pdfFile in pdfFiles)
        {
            text = ExtractTextFromPdf(pdfFile);
            Console.WriteLine($"value: {text}");
        }
    }
    static string ExtractTextFromPdf(string pdfFile)
    {
        string extractedValue = "no match";
        string modified = pdfFile.Insert(pdfFile.Length - 4, "_modified");

        // Open the PDF document
        using (PdfReader reader = new PdfReader(pdfFile))
        using (PdfWriter writer = new PdfWriter(modified)) // Output path
        using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
        {
            // Get the PDF form
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
      
            // Remove text fields temporarily
            foreach (var field in fields)
            {
                if (field.Value is not null)
                {
                    form.RemoveField(field.Key);
                }
            }         
  
            var pageNumber = 1;
            var page = pdfDoc.GetPage(pageNumber);

            // Extract text using iText7
            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
            var text = PdfTextExtractor.GetTextFromPage(page, strategy);

            string pattern = @"ECO_10_\d+\s+(\w+)";
            Match match = Regex.Match(text, pattern);
            
            if (match.Success)
            {
                // Group 1 contains the desired string
                extractedValue = match.Groups[1].Value;
                if (extractedValue.Length < 11)
                {
                    pattern = @"1 2 3 4 5 6 7 8\s+(\d+)";
                    match = Regex.Match(text, pattern);
                    if (match.Success)
                    {
                        // Group 1 contains the desired string
                        extractedValue = "D" + match.Groups[1].Value;
                        if (extractedValue.Length < 11)
                        {
                            Console.WriteLine(text);    
                        }
                    }
                    else
                    {
                        Console.WriteLine(text);
                    }
                }
            }
            else
            {
                extractedValue = "no match";
            }

            // Restore the form fields
            foreach (var field in fields)
            {
                form.AddField(field.Value);
            }
            
        }

        return extractedValue;
    }


}
