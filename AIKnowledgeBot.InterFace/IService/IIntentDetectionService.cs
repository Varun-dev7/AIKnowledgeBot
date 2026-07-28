using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IIntentDetectionService
    {
        Task<IntentResultDto> DetectAsync(string question,IEnumerable<ChatMessage> history);
    }
}
