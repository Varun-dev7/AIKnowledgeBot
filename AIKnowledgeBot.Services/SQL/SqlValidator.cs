using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AIKnowledgeBot.Services.SQL
{
    public class SqlValidator : ISqlValidator
    {
        private static readonly string[] ForbiddenKeywords =
        {
            "INSERT",
            "UPDATE",
            "DELETE",
            "DROP",
            "ALTER",
            "TRUNCATE",
            "EXEC",
            "EXECUTE",
            "MERGE",
            "CREATE",
            "GRANT",
            "REVOKE",
            "DENY",
            "BACKUP",
            "RESTORE"
        };

        public bool IsSafe(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            var normalized = sql.TrimStart().ToUpperInvariant();

            // Allow safe read-only SQL that may start with SELECT, WITH, or DECLARE
            if (!(normalized.StartsWith("SELECT") ||
                  normalized.StartsWith("WITH") ||
                  normalized.StartsWith("DECLARE")))
            {
                return false;
            }

            foreach (var keyword in ForbiddenKeywords)
            {
                if (Regex.IsMatch(
                    normalized,
                    $@"\b{Regex.Escape(keyword)}\b",
                    RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
