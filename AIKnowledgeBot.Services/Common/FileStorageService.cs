using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Common
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".docx",
            ".txt"
        };

        private const long MaxFileSize = 50 * 1024 * 1024;

        public async Task<FileStorageResult> SaveFileAsync(IFormFile file)
        {
            var result = new FileStorageResult();

            if (file == null || file.Length == 0)
            {
                result.Message = "No file selected.";
                return result;
            }

            if (file.Length > MaxFileSize)
            {
                result.Message = "Maximum file size is 50 MB.";
                return result;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                result.Message = "Only PDF, DOCX and TXT files are allowed.";
                return result;
            }

            string folder = extension switch
            {
                ".pdf" => "pdf",
                ".docx" => "docx",
                ".txt" => "txt",
                _ => "others"
            };

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                 folder);

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var storedFileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadFolder, storedFileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            result.Success = true;
            result.Message = "File uploaded successfully.";

            result.StoredFileName = storedFileName;
            result.OriginalFileName = file.FileName;
            result.RelativePath = Path.Combine("uploads", folder, storedFileName).Replace("\\", "/");
            result.Extension = extension;
            result.ContentType = file.ContentType;
            result.FileSize = file.Length;

            return result;
        }
    }
}
