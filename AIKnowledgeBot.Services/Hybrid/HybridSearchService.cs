using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBot.Models.Entities;
namespace AIKnowledgeBot.Services.Hybrid
{
    public class HybridSearchService : IHybridSearchService
    {
        private readonly ISqlService _sqlService;
        private readonly ISemanticSearchService _semanticSearch;
        private readonly IGeminiClient _geminiClient;

        public HybridSearchService(
            ISqlService sqlService,
            ISemanticSearchService semanticSearch,
            IGeminiClient geminiClient)
        {
            _sqlService = sqlService;
            _semanticSearch = semanticSearch;
            _geminiClient = geminiClient;
        }

        public async Task<string> AskAsync(
            string question,
            IEnumerable<ChatMessage> history)
        {
            // SQL Result
            var sqlResult = await _sqlService.AskAsync(question);

            // RAG Result
            var chunks = await _semanticSearch.SearchAsync(question);

            var prompt = BuildPrompt(
                question,
                sqlResult,
                chunks,
                history);

            return await _geminiClient.GenerateContentAsync(prompt);
        }

        private string BuildPrompt(
            string question,
            string sqlResult,
            List<SearchResultDto> chunks,
            IEnumerable<ChatMessage> history)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an Enterprise AI Assistant.");
            sb.AppendLine();
            sb.AppendLine("Answer using BOTH the SQL result and the document context.");
            sb.AppendLine("Do not ignore either source.");
            sb.AppendLine();

            sb.AppendLine("Conversation History:");
            foreach (var h in history.TakeLast(10))
            {
                sb.AppendLine($"{h.Role}: {h.Message}");
            }

            sb.AppendLine();
            sb.AppendLine("SQL Result:");
            sb.AppendLine(sqlResult);

            sb.AppendLine();
            sb.AppendLine("Document Context:");

            foreach (var chunk in chunks)
            {
                sb.AppendLine(
                    $"Page {chunk.Chunk.PageNumber}: {chunk.Chunk.Content}");
            }

            sb.AppendLine();
            sb.AppendLine($"Question: {question}");
            sb.AppendLine();
            sb.AppendLine("Answer:");

            return sb.ToString();
        }
    }
}
