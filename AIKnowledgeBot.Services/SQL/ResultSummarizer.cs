using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.SQL
{
    public class ResultSummarizer : IResultSummarizer
    {
        private readonly IGeminiClient _geminiClient;

        public ResultSummarizer(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<string> SummarizeAsync(
            string question,
            DataTable table)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"User Question: {question}");
            sb.AppendLine();

            sb.AppendLine("SQL Result:");
            sb.AppendLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append($"{column.ColumnName}: {row[column]} | ");
                }

                sb.AppendLine();
            }

            sb.AppendLine();

            sb.AppendLine("""
You are an Enterprise AI Assistant.

Summarize the SQL result.

Rules:

1. Answer naturally.
2. Never mention SQL.
3. Never mention database.
4. If there are multiple rows summarize them.
5. If there are no rows say:
"No records found."
6. Keep the answer concise.
""");

            return await _geminiClient.GenerateContentAsync(sb.ToString());
        }
    }
}
