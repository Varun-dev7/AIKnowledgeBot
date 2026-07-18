using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IKnowledgePipelineService
    {
        Task ProcessDocumentAsync(Guid documentId);
    }
}
