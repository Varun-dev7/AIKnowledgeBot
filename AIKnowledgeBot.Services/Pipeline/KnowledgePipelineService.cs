using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Pipeline
{
    public class KnowledgePipelineService
         : IKnowledgePipelineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentProcessorService _documentProcessor;

        private readonly IChunkService _chunkService;
        private readonly IEmbeddingService _embeddingService;

        private readonly IFilePathProvider _filePathProvider;
        public KnowledgePipelineService(
            IUnitOfWork unitOfWork,
            IDocumentProcessorService documentProcessor,
            IChunkService chunkService,
            IEmbeddingService embeddingService,
            IFilePathProvider filePathProvider)
        {
            _unitOfWork = unitOfWork;
            _documentProcessor = documentProcessor;
            _chunkService = chunkService;
            _embeddingService = embeddingService;
            _filePathProvider = filePathProvider;
        }

        public async Task ProcessDocumentAsync(Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);

            if (document == null)
                throw new Exception("Document not found.");

            try
            {
                document.Status = DocumentStatus.Processing;

                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                // Physical file path
                var physicalPath = _filePathProvider.GetPhysicalPath(document.FilePath);

                // Extract text
                var pages = await _documentProcessor.ProcessAsync(physicalPath);

                // Chunk
                var chunks = _chunkService.CreateChunks(document.Id, pages);

                // Save chunks
                await _unitOfWork.Chunks.AddRangeAsync(chunks);
                await _unitOfWork.SaveChangesAsync();

                // Generate embeddings
                await _embeddingService.GenerateEmbeddingsAsync(document.Id);

                // Completed
                document.Status = DocumentStatus.Completed;
                document.LastProcessedDate = DateTime.UtcNow;

                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                document.Status = DocumentStatus.Failed;

                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                throw;
            }
        }
    }
}
