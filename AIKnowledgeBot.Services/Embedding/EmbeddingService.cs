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

            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);

            if (document == null)
                return;

            document.TotalChunks = chunks.Count;
            document.ProcessedChunks = 0;

            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            var embeddings = new List<DocumentEmbedding>();

            Console.WriteLine($"Chunks Count = {chunks.Count}");

            foreach (var chunk in chunks)
            {
                if (string.IsNullOrWhiteSpace(chunk.Content))
                    continue;

                var vector = await GenerateEmbeddingWithRetryAsync(chunk.Content);

                await Task.Delay(600);

                //embeddings.Add(new DocumentEmbedding
                //{
                //    Id = Guid.NewGuid(),
                //    ChunkId = chunk.Id,
                //    EmbeddingJson = JsonSerializer.Serialize(vector),
                //    Model = "text-embedding-3-small",
                //    CreatedAt = DateTime.UtcNow
                //});
                embeddings.Add(new DocumentEmbedding
                {
                    Id = Guid.NewGuid(),
                    ChunkId = chunk.Id,
                    EmbeddingJson = JsonSerializer.Serialize(vector),
                    Model = "text-embedding-3-small",
                    CreatedAt = DateTime.UtcNow
                });
                // Update Progress
                document.ProcessedChunks++;

                // Save progress every 10 chunks
                if (document.ProcessedChunks % 10 == 0)
                {
                    await _unitOfWork.Documents.UpdateAsync(document);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            if (embeddings.Any())
            {
                await _unitOfWork.Embeddings.AddRangeAsync(embeddings);
            }

            // Final Progress Update
            document.ProcessedChunks = document.TotalChunks;

            await _unitOfWork.Documents.UpdateAsync(document);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<float[]> GenerateEmbeddingWithRetryAsync(string text)
        {
            const int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await _geminiClient.GenerateEmbeddingAsync(text);
                }
                catch (Exception ex)
                {
                    var isRateLimit =
                        ex.Message.Contains("429") ||
                        ex.Message.Contains("Too Many Requests");

                    if (!isRateLimit || attempt == maxRetries)
                        throw;

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));

                    Console.WriteLine(
                        $"429 received. Retry {attempt}/{maxRetries}. Waiting {delay.TotalSeconds} seconds...");

                    await Task.Delay(delay);
                }
            }

            throw new Exception("Embedding generation failed.");
        }
    }
}
