using AIKnowledgeBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.DTOs
{
    public class IntentResultDto
    {
        public QueryIntent Intent { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
