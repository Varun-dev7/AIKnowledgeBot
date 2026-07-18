using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace AIKnowledgeBot.Services.Extractors
{
    public class PdfTextExtractor : IDocumentTextExtractor
    {
        public bool CanHandle(string extension)
        {
            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<DocumentPage>> ExtractAsync(string filePath)
        {
            var pages = new List<DocumentPage>();

            using var document = PdfDocument.Open(filePath);

            foreach (var page in document.GetPages())
            {
                pages.Add(new DocumentPage
                {
                    PageNumber = page.Number,
                    Text = page.Text
                });
            }

            return await Task.FromResult(pages);
        }
    }
}
