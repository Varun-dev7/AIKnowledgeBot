using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Common;
using AIKnowledgeBot.Models.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AIKnowledgeBot.Services.Embedding
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeminiClient _geminiClient;

        public EmbeddingService(
            IUnitOfWork unitOfWork,
            IGeminiClient geminiClient)
        {
            _unitOfWork = unitOfWork;
            _geminiClient = geminiClient;
        }

        public async Task GenerateEmbeddingsAsync(Guid documentId)
        {
            var chunks = await _unitOfWork.Chunks.GetByDocumentIdAsync(documentId);

            if (chunks == null || chunks.Count == 0)
                return;

            var embeddings = new List<DocumentEmbedding>();

            foreach (var chunk in chunks)
            {
                if (string.IsNullOrWhiteSpace(chunk.Content))
                    continue;

                var vector = await _geminiClient.GenerateEmbeddingAsync(chunk.Content);

                embeddings.Add(new DocumentEmbedding
                {
                    Id = Guid.NewGuid(),
                    ChunkId = chunk.Id,
                    EmbeddingJson = JsonSerializer.Serialize(vector),
                    Model = "gemini-embedding-001",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (embeddings.Any())
            {
                await _unitOfWork.Embeddings.AddRangeAsync(embeddings);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
