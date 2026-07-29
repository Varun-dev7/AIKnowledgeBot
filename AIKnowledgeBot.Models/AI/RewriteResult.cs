using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.AI
{
    public class RewriteResult
    {
        public string Question { get; set; } = string.Empty;

        public bool IsFollowUp { get; set; }
    }
}
