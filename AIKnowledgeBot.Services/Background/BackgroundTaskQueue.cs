using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Background
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Guid> _queue;

        public BackgroundTaskQueue()
        {
            _queue = Channel.CreateUnbounded<Guid>();
        }

        public async ValueTask QueueDocumentAsync(Guid documentId)
        {
            await _queue.Writer.WriteAsync(documentId);
        }

        public async ValueTask<Guid> DequeueDocumentAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
