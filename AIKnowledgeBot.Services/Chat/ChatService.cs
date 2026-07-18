using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly ISemanticSearchService _semanticSearch;
        private readonly IGeminiClient _geminiClient;

        public ChatService(
            ISemanticSearchService semanticSearch,
            IGeminiClient geminiClient)
        {
            _semanticSearch = semanticSearch;
            _geminiClient = geminiClient;
        }

        public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
        {
            var chunks = await _semanticSearch.SearchAsync(request.Question);

            if (chunks == null || !chunks.Any())
            {
                return new ChatResponseDto
                {
                    Answer = "I couldn't find any relevant information in the uploaded documents."
                };
            }

            var prompt = BuildPrompt(request.Question, chunks);

            var answer = await _geminiClient.GenerateAnswerAsync(prompt);

            var response = new ChatResponseDto
            {
                Answer = answer
            };

            foreach (var chunk in chunks)
            {
                response.Sources.Add(new SourceDto
                {
                    DocumentId = chunk.DocumentId,
                    DocumentName = chunk.Document?.Title ?? "Unknown Document",
                    PageNumber = chunk.PageNumber,
                    Score = 0
                });
            }

            return response;
        }

        private string BuildPrompt(string question, List<DocumentChunk> chunks)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an Enterprise AI Knowledge Assistant.");

            sb.AppendLine();

            sb.AppendLine("Use ONLY the information provided in the context.");

            sb.AppendLine("Do NOT use your own knowledge.");

            sb.AppendLine("Do NOT summarize.");

            sb.AppendLine("Do NOT shorten the answer.");

            sb.AppendLine("If the context contains multiple relevant sentences, include ALL of them.");

            sb.AppendLine("Return a complete and detailed answer.");

            sb.AppendLine();

            sb.AppendLine("Context:");
            sb.AppendLine("--------------------------------");

            foreach (var chunk in chunks)
            {
                sb.AppendLine(chunk.Content);
                sb.AppendLine();
            }

            sb.AppendLine("--------------------------------");

            sb.AppendLine();

            sb.AppendLine($"Question: {question}");

            sb.AppendLine();

            sb.AppendLine("Answer:");

            return sb.ToString();
        }
    }
}
