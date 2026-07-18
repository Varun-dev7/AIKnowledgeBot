using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs.Document;
using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Chunking
{
    public class ChunkService : IChunkService
    {
        private const int ChunkSize = 1000;

        private const int ChunkOverlap = 150;

        public List<DocumentChunk> CreateChunks(
            Guid documentId,
            List<DocumentPage> pages)
        {
            var chunks = new List<DocumentChunk>();

            int chunkIndex = 1;

            foreach (var page in pages)
            {
                var text = page.Text;

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                int start = 0;

                while (start < text.Length)
                {
                    int length = Math.Min(ChunkSize, text.Length - start);

                    var chunkText = text.Substring(start, length);

                    chunks.Add(new DocumentChunk
                    {
                        Id = Guid.NewGuid(),

                        DocumentId = documentId,

                        PageNumber = page.PageNumber,

                        ChunkIndex = chunkIndex++,

                        Content = chunkText,

                        CreatedAt = DateTime.UtcNow
                    });

                    if (start + length >= text.Length)
                        break;

                    start += ChunkSize - ChunkOverlap;
                }
            }

            return chunks;
        }
    }
}
