using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
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

        private string BuildPrompt(string question, List<Models.Entities.DocumentChunk> chunks)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an Enterprise AI Knowledge Assistant.");

            sb.AppendLine();

            sb.AppendLine("Answer ONLY from the provided context.");

            sb.AppendLine();

            sb.AppendLine("If the answer is not present, reply:");

            sb.AppendLine("\"I couldn't find that information in the uploaded documents.\"");

            sb.AppendLine();

            sb.AppendLine("CONTEXT");

            sb.AppendLine("-----------------------------------------");

            foreach (var chunk in chunks)
            {
                sb.AppendLine(chunk.Content);

                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------");

            sb.AppendLine();

            sb.AppendLine("QUESTION:");

            sb.AppendLine(question);

            return sb.ToString();
        }
    }
}
