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
                // ============================
                // STEP 1 : Extracting
                // ============================
                document.Status = DocumentStatus.Extracting;
                document.ProcessingMessage = "Extracting document...";
                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                var physicalPath = _filePathProvider.GetPhysicalPath(document.FilePath);

                var pages = await _documentProcessor.ProcessAsync(physicalPath);

                // ============================
                // STEP 2 : Chunking
                // ============================
                document.Status = DocumentStatus.Chunking;
                document.ProcessingMessage = "Creating chunks...";
                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                var chunks = _chunkService.CreateChunks(document.Id, pages);

                document.TotalChunks = chunks.Count;
                document.ProcessedChunks = 0;

                await _unitOfWork.Chunks.AddRangeAsync(chunks);
                await _unitOfWork.SaveChangesAsync();

                // ============================
                // STEP 3 : Embedding
                // ============================
                document.Status = DocumentStatus.Embedding;
                document.ProcessingMessage = "Generating embeddings...";
                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                await _embeddingService.GenerateEmbeddingsAsync(document.Id);

                // ============================
                // STEP 4 : Completed
                // ============================
                document.Status = DocumentStatus.Completed;
                document.ProcessingMessage = "Completed";
                document.LastProcessedDate = DateTime.UtcNow;

                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                document.Status = DocumentStatus.Failed;
                document.ProcessingMessage = ex.Message;

                await _unitOfWork.Documents.UpdateAsync(document);
                await _unitOfWork.SaveChangesAsync();

                throw;
            }
        }
    }
}
