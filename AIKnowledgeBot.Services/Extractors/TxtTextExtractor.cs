using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Extractors
{
    public class TxtTextExtractor : IDocumentTextExtractor
    {
        public bool CanHandle(string extension)
        {
            return extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<DocumentPage>> ExtractAsync(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
