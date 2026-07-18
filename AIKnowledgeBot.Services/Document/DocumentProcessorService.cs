using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Document
{
    public class DocumentProcessorService : IDocumentProcessorService
    {
        private readonly IEnumerable<IDocumentTextExtractor> _extractors;

        public DocumentProcessorService(IEnumerable<IDocumentTextExtractor> extractors)
        {
            _extractors = extractors;
        }

        public async Task<List<DocumentPage>> ProcessAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            var extractor = _extractors.FirstOrDefault(x => x.CanHandle(extension));

            if (extractor == null)
                throw new NotSupportedException($"No extractor found for {extension}");

            return await extractor.ExtractAsync(filePath);
        }
    }
}
