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

Your job is ONLY to determine whether the latest question depends on the previous conversation.

Do NOT answer the question.
Do NOT add information.
Do NOT change the user's intent.

Conversation History:

{historyText}

Latest Question:

{question}

Rules:

1. If the latest question is independent, set IsFollowUp = false.
2. If it depends on previous conversation, set IsFollowUp = true and rewrite it into a standalone question.
3. If the latest question is already complete, return it exactly as written.
4. If the latest question is a topic, title, heading, keyword, name, or standalone search phrase, DO NOT rewrite it.
5. Never expand the question.
6. Never make the question longer.
7. Never add words that the user did not write.
8. Rewrite only when required to resolve references to previous conversation.

Return ONLY valid JSON in this format:

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
