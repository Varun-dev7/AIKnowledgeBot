using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueDocumentAsync(Guid documentId);

        ValueTask<Guid> DequeueDocumentAsync(CancellationToken cancellationToken);
    }
}
