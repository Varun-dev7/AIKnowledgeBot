using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface ISemanticSearchService
    {
        Task<List<DocumentChunk>> SearchAsync(string question, int topK = 10);
    }
}
