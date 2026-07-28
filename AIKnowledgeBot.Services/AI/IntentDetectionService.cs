using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIKnowledgeBot.Models.Entities;

namespace AIKnowledgeBot.Services.AI
{
    public class IntentDetectionService : IIntentDetectionService
    {
        private readonly IGeminiClient _geminiClient;

        public IntentDetectionService(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<IntentResultDto> DetectAsync(
            string question,
            IEnumerable<ChatMessage> history)
        {
            var historyText = string.Join(
                Environment.NewLine,
                history.Select(x => $"{x.Role}: {x.Message}"));

            var prompt = $"""
You are an Enterprise AI Intent Detection System.

Conversation History:
{historyText}

Current Question:
{question}

Classify the user's question into ONLY ONE category.

1. Document
Questions ONLY about:
- Uploaded books
- PDF content
- Notes
- Chapters
- Definitions
- Concepts
- Exercises

Examples:
- What is Triangle?
- Explain Chapter 5.
- Define Matter.

----------------------------

2. Sql
Questions ONLY about structured database information.

Examples:
- Show all uploaded books.
- List categories.
- Show all documents uploaded today.
- How many books are uploaded?
- Show all Chemistry books.

----------------------------

3. Hybrid

Questions requiring BOTH SQL data AND document knowledge.

Examples:
- Show Chemistry books uploaded this month and explain Chapter 1.
- List uploaded Mathematics books and summarize the first chapter.
- Which books were uploaded this week and what do they teach?
- Show Physics books uploaded today and explain Newton's Laws.

----------------------------

4. General

Greetings and casual conversation.

Examples:
- Hello
- Hi
- Good Morning

----------------------------

Return ONLY valid JSON.

Example:
       "intent":"Hybrid",
    "reason":"Requires both SQL data and document content."
""";

            var result = await _geminiClient.GenerateContentAsync(prompt);

            try
            {
                using var doc = JsonDocument.Parse(result);

                var intentString = doc.RootElement
                    .GetProperty("intent")
                    .GetString();

                var reason = doc.RootElement
                    .GetProperty("reason")
                    .GetString() ?? "";

                return new IntentResultDto
                {
                    Intent = Enum.Parse<QueryIntent>(intentString!, true),
                    Reason = reason
                };
            }
            catch
            {
                return new IntentResultDto
                {
                    Intent = QueryIntent.Document,
                    Reason = "Default fallback."
                };
            }
        }
    }
}
