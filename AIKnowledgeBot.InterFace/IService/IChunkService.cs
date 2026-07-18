using AIKnowledgeBot.Models.DTOs.Document;
using AIKnowledgeBot.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IChunkService
    {
        List<DocumentChunk> CreateChunks(
            Guid documentId,
            List<DocumentPage> pages);
    }
}
