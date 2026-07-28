using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.SQL
{
    public class SqlPromptBuilder
    {
        public string Build(string question, string schema)
        {
            var sb = new StringBuilder();

            sb.AppendLine("You are an expert SQL Server developer.");
            sb.AppendLine();

            sb.AppendLine("Database Schema:");
            sb.AppendLine("--------------------------------");
            sb.AppendLine(schema);
            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            sb.AppendLine("Rules:");
            sb.AppendLine("1. Generate EXACTLY ONE read-only SQL Server query.");
            sb.AppendLine("2. The query MUST start with SELECT.");
            sb.AppendLine("3. Do NOT use DECLARE.");
            sb.AppendLine("4. Do NOT use variables.");
            sb.AppendLine("5. Do NOT use WITH (CTE).");
            sb.AppendLine("6. Do NOT use temporary tables.");
            sb.AppendLine("7. Do NOT use dynamic SQL.");
            sb.AppendLine("8. Do NOT use EXEC or stored procedures.");
            sb.AppendLine("9. Do NOT use INSERT, UPDATE, DELETE, MERGE, DROP, ALTER, TRUNCATE, CREATE.");
            sb.AppendLine("10. Use only tables and columns from the provided schema.");
            sb.AppendLine("11. Use JOIN when needed.");
            sb.AppendLine("12. Use GETDATE() directly instead of DECLARE variables.");
            sb.AppendLine("13. Return ONLY SQL.");
            sb.AppendLine("14. Do NOT wrap the SQL in markdown.");
            sb.AppendLine("15. Do NOT explain the query.");
            sb.AppendLine();

            sb.AppendLine($"Question: {question}");
            sb.AppendLine();
            sb.AppendLine("SQL:");

            return sb.ToString();
        }
    }
}
