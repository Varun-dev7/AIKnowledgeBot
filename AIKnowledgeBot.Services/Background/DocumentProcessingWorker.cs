using AIKnowledgeBot.InterFace.IService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Background
{
    public class DocumentProcessingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<DocumentProcessingWorker> _logger;

        public DocumentProcessingWorker(
            IServiceScopeFactory scopeFactory,
            IBackgroundTaskQueue taskQueue,
            ILogger<DocumentProcessingWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Document Processing Worker Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var documentId =
                        await _taskQueue.DequeueDocumentAsync(stoppingToken);

                    using var scope = _scopeFactory.CreateScope();

                    var pipeline =
                        scope.ServiceProvider.GetRequiredService<IKnowledgePipelineService>();

                    _logger.LogInformation(
                        "Processing Document {DocumentId}",
                        documentId);

                    await ProcessWithRetryAsync(pipeline, documentId, stoppingToken);

                    _logger.LogInformation(
                        "Completed Document {DocumentId}",
                        documentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background Worker Error");
                }
            }
        }

        private async Task ProcessWithRetryAsync(
    IKnowledgePipelineService pipeline,
    Guid documentId,
    CancellationToken cancellationToken)
        {
            const int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await pipeline.ProcessDocumentAsync(documentId);

                    return;
                }
                catch (Exception ex)
                {
                    var isRateLimit =
                        ex.Message.Contains("429") ||
                        ex.Message.Contains("Too Many Requests");

                    if (!isRateLimit || attempt == maxRetries)
                    {
                        throw;
                    }

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));

                    _logger.LogWarning(
                        "429 received while processing document {DocumentId}. Retry {Attempt}/{Max}. Waiting {Delay} seconds.",
                        documentId,
                        attempt,
                        maxRetries,
                        delay.TotalSeconds);

                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
    }
}
