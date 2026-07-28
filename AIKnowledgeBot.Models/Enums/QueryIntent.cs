using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Models.Enums
{
    public enum QueryIntent
    {
        Document = 1,
        Sql = 2,
        Hybrid = 3,
        General = 4
    }
}
