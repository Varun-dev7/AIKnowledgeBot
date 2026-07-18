using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Common
{
    public class FilePathProvider : IFilePathProvider
    {
        public string GetPhysicalPath(string relativePath)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        }
    }
}
