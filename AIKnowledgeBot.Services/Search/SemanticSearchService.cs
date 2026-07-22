using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Search
{
    public class SemanticSearchService : ISemanticSearchService
    {
        private readonly IGeminiClient _geminiClient;
        private readonly ISimilarityService _similarityService;
        private readonly IUnitOfWork _unitOfWork;

        public SemanticSearchService(
            IGeminiClient geminiClient,
            ISimilarityService similarityService,
            IUnitOfWork unitOfWork)
        {
            _geminiClient = geminiClient;
            _similarityService = similarityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SearchResultDto>> SearchAsync(string question, int topK = 3)
        {
            // Generate embedding for the user's question
            var questionEmbedding =
                await _geminiClient.GenerateEmbeddingAsync(question);

            // Load all stored embeddings
            var embeddings =
                await _unitOfWork.Embeddings.GetAllAsync();

            var results = new List<SearchResultDto>();

            foreach (var embedding in embeddings)
            {
                var vector = JsonSerializer.Deserialize<float[]>(embedding.EmbeddingJson);

                if (vector == null || embedding.Chunk == null)
                    continue;

                var score = _similarityService.CosineSimilarity(
                    questionEmbedding,
                    vector);

                results.Add(new SearchResultDto
                {
                    Chunk = embedding.Chunk,
                    Score = score
                });
            }

            return results
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();
        }
    }
}
