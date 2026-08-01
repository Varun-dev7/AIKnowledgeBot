using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Models.Enums;
using AIKnowledgeBot.Services.IntentDetection;
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
        private readonly IIntentDetectionService _intentDetectionService;
        private readonly ISqlService _sqlService;
        private readonly IHybridSearchService _hybridSearchService;
        public ChatService(
            ISemanticSearchService semanticSearch,
            IGeminiClient geminiClient,
            IUnitOfWork unitOfWork,
            IQueryRewriteService queryRewriteService,
            IIntentDetectionService intentDetectionService,
            ISqlService sqlService,
            IHybridSearchService hybridSearchService)
        {
            _semanticSearch = semanticSearch;
            _geminiClient = geminiClient;
            _unitOfWork = unitOfWork;
            _queryRewriteService = queryRewriteService;
            _intentDetectionService = intentDetectionService;
            _sqlService = sqlService;
            _hybridSearchService = hybridSearchService;
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

            var history = await _unitOfWork.ChatMessages.GetByConversationIdAsync(conversation.Id);

            var rewriteResult = await _queryRewriteService.RewriteAsync(request.Question,history);
            Console.WriteLine("=================================");
            Console.WriteLine($"Original Question : {request.Question}");
            Console.WriteLine($"Rewritten Question: {rewriteResult.Question}");
            Console.WriteLine($"IsFollowUp: {rewriteResult.IsFollowUp}");
            Console.WriteLine("=================================");
            var intent = await _intentDetectionService.DetectAsync(rewriteResult.Question,history);
            Console.WriteLine($"Intent: {intent.Intent}");
            List<SearchResultDto> chunks = new();

            string answer = string.Empty;

            switch (intent.Intent)
            {
                case QueryIntent.Document:

                    // Existing RAG Flow
                    chunks = await _semanticSearch.SearchAsync(rewriteResult.Question);
                    Console.WriteLine("===== SEARCH RESULTS =====");

                    foreach (var chunk in chunks)
                    {
                        Console.WriteLine($"Score : {chunk.Score:F4}");
                        Console.WriteLine(chunk.Chunk.Content);
                        Console.WriteLine("---------------------------");
                    }
                    if (!chunks.Any())
                    {
                        answer = "I couldn't find any relevant information in the uploaded documents.";
                        break;
                    }

                    var promptHistory = rewriteResult.IsFollowUp? history.OrderByDescending(x => x.CreatedAt).Take(6).Reverse().ToList(): new List<ChatMessage>();

                    var prompt = BuildPrompt(rewriteResult.Question,chunks,promptHistory);
                    Console.WriteLine("===== INTENT PROMPT =====");
                    Console.WriteLine(prompt);
                    Console.WriteLine("=========================");
                    answer = await _geminiClient.GenerateContentAsync(prompt);
                    Console.WriteLine("===== INTENT RESPONSE =====");
                    Console.WriteLine(answer);
                    Console.WriteLine("===========================");

                    break;

                case QueryIntent.Sql:

                    answer = await _sqlService.AskAsync(rewriteResult.Question);

                    break;

                case QueryIntent.Hybrid:

                    // Next feature
                    // SQL + RAG
                    answer = await _hybridSearchService.AskAsync(rewriteResult.Question, history);
                    break;

                //case QueryIntent.General:

                //    answer = await _geminiClient.GenerateContentAsync(rewriteResult.Question);

                //    break;

            }

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

            if (chunks.Any())
            {
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
            }

            return response;
        }

        private string BuildPrompt(string question, List<SearchResultDto> chunks, List<ChatMessage> history)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an Enterprise AI Knowledge Assistant.");
            sb.AppendLine();

            sb.AppendLine("Rules:");
            sb.AppendLine("1. Answer ONLY using the provided context.");
            sb.AppendLine("2. Never use outside knowledge.");
            sb.AppendLine("3. If the answer exists in the context, copy the relevant sentence(s) exactly as written.");
            sb.AppendLine("4. If the context contains only a partial answer, answer using only the available information.");
            sb.AppendLine("5. Never invent facts, formulas, definitions, or explanations.");
            sb.AppendLine("6. Never mention document names, page numbers, chunk numbers, or phrases like 'According to the document', 'The answer appears on Page...', or 'The provided document'.");
            sb.AppendLine("7. Do not tell the user where the answer was found. Source information is provided separately by the application.");
            sb.AppendLine("8. If the answer is not found in the context, reply exactly:");
            sb.AppendLine("\"I couldn't find this information in the uploaded documents.\"");
            sb.AppendLine("9. Keep the response clear, natural, and professional.");
            sb.AppendLine("10. Detect the user's intent before answering.\r\n\r\n- If the user asks for only the final answer, return only the final answer.\r\n- If the user asks \"Explain\", \"Why\", \"How\", or \"Solve\", provide a detailed explanation.\r\n- If the user asks \"Where is...\", answer briefly and let the application show the source separately.\r\n- Keep answers concise unless the user explicitly requests more detail.");
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
                sb.AppendLine($"Document: {item.Chunk.Document?.Title}");
                sb.AppendLine($"Page: {item.Chunk.PageNumber}");
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
