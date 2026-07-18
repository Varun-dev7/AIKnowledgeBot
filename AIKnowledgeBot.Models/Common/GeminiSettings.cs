using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Common
{
    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;

        public string ChatModel { get; set; } = string.Empty;

        public string EmbeddingModel { get; set; } = string.Empty;
    }
}
