using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Pdf.Utility
{
    class Program
    {
        private const string PdfRoot = "C:/pdfs/";
        private const string PdfSplitDir = "C:/pdfs/split";
        private const string PdfMergedDir = "C:/pdfs/split";

        static void Main(string[] args)
        {
            Console.WriteLine("PDF Util");
            Console.WriteLine("Press 1 for split, Press 2 for merge");
            var mode = Console.ReadLine();

            if (mode == "1")
                Split();
            else if (mode == "2")
            {
                Merge();
            }
            else
            {
                Console.WriteLine("Try again");
                Console.ReadLine();
            }
        }

        public static void Split()
        {
            Console.WriteLine("Enter filename:");
            var filename = Console.ReadLine();

            // Get a fresh copy of the sample PDF file
            File.Copy(Path.Combine(PdfRoot, filename), Path.Combine(Directory.GetCurrentDirectory(), filename), true);

            // Open the file
            var inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

            var name = Path.GetFileNameWithoutExtension(filename);
            for (var idx = 0; idx < inputDocument.PageCount; idx++)
            {
                // Create new document
                var outputDocument = new PdfDocument { Version = inputDocument.Version };
                outputDocument.Info.Title = $"Page {idx + 1} of {inputDocument.Info.Title}";
                outputDocument.Info.Creator = inputDocument.Info.Creator;

                // Add the page and save it
                outputDocument.AddPage(inputDocument.Pages[idx]);
                outputDocument.Save($"{PdfSplitDir}/{name} - Page {idx + 1}.pdf");
            }
        }

        public static void Merge()
        {
            Console.WriteLine("Enter filename (without page number):");
            var filename = Console.ReadLine();

            var files = GetFiles(filename);
            Console.WriteLine($"Merging files: {string.Join(",", files)}");

            // Open the output document
            var outputDocument = new PdfDocument();

            // Iterate files
            foreach (string file in files)
            {
                // Open the document to import pages from it.
                var inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                var count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    var page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            var saveAsFilename = $"{PdfMergedDir}/{filename}.merged.pdf";
            outputDocument.Save(saveAsFilename);
            // ...and start a viewer.
            Process.Start(saveAsFilename);
        }

        public static string[] GetFiles(string filename)
        {
            var fileNames = new List<string>();

            Console.WriteLine("Enter total pages:");
            var pages = int.Parse(Console.ReadLine());

            for (var i = 0; i < pages; i++)
            {
                var tempPageName = $"{filename} - Page {i + 1}.pdf";
                var tempPagePage = Path.Combine(PdfSplitDir, tempPageName);

                if (File.Exists(tempPagePage))
                {
                    fileNames.Add(tempPagePage);
                }
                else
                {
                    Console.WriteLine($"Page {tempPageName} does not exist, exiting");
                    Console.ReadLine();
                }
            }

            return fileNames.ToArray();
        }
    }
}
