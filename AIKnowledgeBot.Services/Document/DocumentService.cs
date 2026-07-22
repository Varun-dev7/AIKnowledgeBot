using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Common;
using AIKnowledgeBot.Models.DTOs.Document;
using AIKnowledgeBot.Models.Enums;
using AIKnowledgeBot.Services.Background;
using AIKnowledgeBot.Services.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public DocumentService(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public async Task<ApiResponse<DocumentResponseDto>> UploadAsync(UploadDocumentDto dto)
        {
            var response = new ApiResponse<DocumentResponseDto>();

            // Validate Category
            if (!await _unitOfWork.Categories.ExistsAsync(dto.CategoryId))
            {
                response.Success = false;
                response.Message = "Category not found.";
                return response;
            }

            // Validate Subject
            if (!await _unitOfWork.Subjects.ExistsAsync(dto.SubjectId))
            {
                response.Success = false;
                response.Message = "Subject not found.";
                return response;
            }

            // Save File
            var fileResult = await _fileStorageService.SaveFileAsync(dto.File);

            if (!fileResult.Success)
            {
                response.Success = false;
                response.Message = fileResult.Message;
                return response;
            }

            // Create Document Entity
            var document = new Models.Entities.Document
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,

                CategoryId = dto.CategoryId,
                SubjectId = dto.SubjectId,

                OriginalFileName = fileResult.OriginalFileName,
                StoredFileName = fileResult.StoredFileName,

                Extension = fileResult.Extension,
                ContentType = fileResult.ContentType,

                FileSize = fileResult.FileSize,
                FilePath = fileResult.RelativePath,

                Status = DocumentStatus.Pending,

                UploadedBy = "Admin",

                UploadedDate = DateTime.UtcNow,

                IsActive = true
            };

            await _unitOfWork.Documents.AddAsync(document);

            await _unitOfWork.SaveChangesAsync();
            await _backgroundTaskQueue.QueueDocumentAsync(document.Id);

            response.Success = true;
            response.Message = "Document uploaded successfully.";

            response.Data = new DocumentResponseDto
            {
                Id = document.Id,
                Title = document.Title,
                OriginalFileName = document.OriginalFileName,
                StoredFileName = document.StoredFileName,
                FilePath = document.FilePath,
                Status = document.Status.ToString(),
                UploadedDate = document.UploadedDate
            };

            return response;
        }

        public async Task<ApiResponse<List<DocumentResponseDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<DocumentResponseDto>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
