using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface IResultSummarizer
    {
        Task<string> SummarizeAsync(
            string question,
            DataTable table);
    }
}
