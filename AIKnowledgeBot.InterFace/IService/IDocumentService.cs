using AIKnowledgeBot.Models.Common;
using AIKnowledgeBot.Models.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IDocumentService
    {
        Task<ApiResponse<DocumentResponseDto>> UploadAsync(UploadDocumentDto dto);

        Task<ApiResponse<List<DocumentResponseDto>>> GetAllAsync();

        Task<ApiResponse<DocumentResponseDto>> GetByIdAsync(Guid id);

        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
