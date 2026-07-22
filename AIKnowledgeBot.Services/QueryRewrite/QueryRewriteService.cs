using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Services.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<string> RewriteAsync(
     string question,
    IEnumerable<ChatMessage> history)
        {
            var historyText = string.Join(
                Environment.NewLine,
                history.Select(x => $"{x.Role}: {x.Message.Trim()}"));

            var prompt = $@"
You are a query rewriting assistant.

Your job is NOT to answer the question.

Rewrite the user's latest question into a complete standalone question.

Conversation:

{historyText}

Latest Question:

{question}

Rules:
- Keep the original meaning.
- Include the previous topic if needed.
- Return ONLY the rewritten question.
";

            var rewrittenQuestion = await _geminiClient.GenerateAnswerAsync(prompt);

            return rewrittenQuestion.Trim();
        }
    }
}
