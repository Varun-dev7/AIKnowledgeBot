using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.SQL
{
    public class SqlService : ISqlService
    {
        private readonly ISqlGenerator _generator;
        private readonly ISqlExecutor _executor;
        private readonly IResultSummarizer _summarizer;

        public SqlService(
            ISqlGenerator generator,
            ISqlExecutor executor,
            IResultSummarizer summarizer)
        {
            _generator = generator;
            _executor = executor;
            _summarizer = summarizer;
        }

        public async Task<string> AskAsync(string question)
        {
            var sql = await _generator.GenerateSqlAsync(question);

            var table = await _executor.ExecuteAsync(sql);

            var answer = await _summarizer.SummarizeAsync(question, table);

            return answer;
        }
    }
}
