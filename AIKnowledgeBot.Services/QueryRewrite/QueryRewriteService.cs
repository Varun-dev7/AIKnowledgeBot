using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.AI;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Services.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.QueryRewrite
{
    public class QueryRewriteService : IQueryRewriteService
    {
        private readonly IGeminiClient _geminiClient;

        public QueryRewriteService(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<RewriteResult> RewriteAsync(
     string question,
    IEnumerable<ChatMessage> history)
        {
            var historyText = string.Join(
                Environment.NewLine,
                history.Select(x => $"{x.Role}: {x.Message.Trim()}"));

            var prompt = $@"
You are a query rewriting assistant.

Your job is NOT to answer the question.

Determine whether the latest question depends on the previous conversation.

Return ONLY valid JSON.

Conversation:

{historyText}

Latest Question:

{question}

Rules:

- If the latest question is independent, set IsFollowUp=false.
- If it refers to previous messages (it, this, that, previous answer, explain more, give examples, continue, etc.), set IsFollowUp=true.
- Rewrite the question into a complete standalone question.

Return EXACTLY:

{{
  ""Question"": ""..."",
  ""IsFollowUp"": true
}}
";

            var json =
          await _geminiClient.GenerateAnswerAsync(prompt);

            var result = JsonSerializer.Deserialize<RewriteResult>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result ?? new RewriteResult
            {
                Question = question,
                IsFollowUp = false
            };
        }
    }
}
