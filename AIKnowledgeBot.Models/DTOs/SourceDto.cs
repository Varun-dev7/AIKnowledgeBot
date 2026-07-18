using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.DTOs
{
    public class SourceDto
    {
        public Guid DocumentId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public int PageNumber { get; set; }

        public double Score { get; set; }
    }
}
