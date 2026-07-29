using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBot.Models.AI;
namespace AIKnowledgeBot.InterFace.IService
{
    public interface IQueryRewriteService
    {
        Task<RewriteResult> RewriteAsync(string question,IEnumerable<ChatMessage> history);
    }
}
