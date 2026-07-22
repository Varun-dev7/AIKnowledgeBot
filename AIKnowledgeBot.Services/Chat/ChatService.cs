using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Models.Enums;
using AIKnowledgeBot.Services.QueryRewrite;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRewriteService _queryRewriteService;
        public ChatService(
            ISemanticSearchService semanticSearch,
            IGeminiClient geminiClient,
            IUnitOfWork unitOfWork,
            IQueryRewriteService queryRewriteService)
        {
            _semanticSearch = semanticSearch;
            _geminiClient = geminiClient;
            _unitOfWork = unitOfWork;
            _queryRewriteService = queryRewriteService;
        }

        public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
        {
            Conversation conversation;

            if (request.ConversationId.HasValue)
            {
                conversation = await _unitOfWork.Conversations
                    .GetByIdAsync(request.ConversationId.Value)
                    ?? throw new Exception("Conversation not found.");
            }
            else
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Title = request.Question.Length > 50
                        ? request.Question.Substring(0, 50)
                        : request.Question,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Conversations.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.ChatMessages.AddAsync(new ChatMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = ChatRole.User,
                Message = request.Question,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();

            var history = await _unitOfWork.ChatMessages
     .GetByConversationIdAsync(conversation.Id);

            var rewrittenQuestion =await _queryRewriteService.RewriteAsync(request.Question,history);

            var chunks = await _semanticSearch.SearchAsync(rewrittenQuestion);

            if (chunks == null || !chunks.Any())
            {
                return new ChatResponseDto
                {
                    Answer = "I couldn't find any relevant information in the uploaded documents."
                };
            }

            var prompt = BuildPrompt(rewrittenQuestion, chunks, history);
            Console.WriteLine(prompt);
            var answer = await _geminiClient.GenerateAnswerAsync(prompt);

            // Save Assistant Message
            await _unitOfWork.ChatMessages.AddAsync(new ChatMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = ChatRole.Assistant,
                Message = answer,
                CreatedAt = DateTime.UtcNow
            });

            conversation.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var response = new ChatResponseDto
            {
                ConversationId = conversation.Id,
                Answer = answer
            };

            foreach (var item in chunks)
            {
                Console.WriteLine("================================");
                Console.WriteLine(item.Chunk.Content);
                response.Sources.Add(new SourceDto
                {
                    DocumentId = item.Chunk.DocumentId,
                    DocumentName = item.Chunk.Document?.Title ?? "Unknown Document",
                    PageNumber = item.Chunk.PageNumber,
                    Score = Math.Round(item.Score, 4)
                });
            }

            return response;
        }

        private string BuildPrompt(string question, List<SearchResultDto> chunks, List<ChatMessage> history)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an Enterprise AI Knowledge Assistant.");

            sb.AppendLine();

            sb.AppendLine("Rules:");

            sb.AppendLine("1. Answer ONLY from the provided context.");

            sb.AppendLine("2. Never use outside knowledge.");

            sb.AppendLine("3. Answer ONLY using the provided context.");
            sb.AppendLine("\"If the context contains a partial answer, answer using the available information.\"");

            sb.AppendLine("4. If the user asks 'What is...', explain the concept.");

            sb.AppendLine("5. Only reply \"I couldn't find this information in the uploaded documents.\"\r\n   if there is absolutely no relevant information in the context.");

            sb.AppendLine("6. If both are asked, explain the concept first and then mention the page number.");

            sb.AppendLine("7. If multiple pages contain relevant information, mention all relevant page numbers.");

            sb.AppendLine("8. Quote important definitions exactly when possible.");

            sb.AppendLine("9. Do not invent information.");

            sb.AppendLine();

            // ===========================
            // Conversation History
            // ===========================
            sb.AppendLine("Conversation History");
            sb.AppendLine("--------------------------------");

            foreach (var message in history.OrderByDescending(x => x.CreatedAt).Take(10).Reverse())
            {
                sb.AppendLine($"{message.Role}: {message.Message}");
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            sb.AppendLine("Context:");
            sb.AppendLine("--------------------------------");

            foreach (var item in chunks)
            {
                sb.AppendLine($"Page {item.Chunk.PageNumber}:");
                sb.AppendLine(item.Chunk.Content);
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
