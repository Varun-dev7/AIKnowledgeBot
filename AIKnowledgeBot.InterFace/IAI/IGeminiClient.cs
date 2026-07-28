using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IAI
{
    public interface IGeminiClient
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<string> GenerateAnswerAsync(string prompt);
        Task<string> GenerateContentAsync(string prompt);
    }
}
