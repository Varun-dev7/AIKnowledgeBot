using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.SQL
{
    public class SqlGenerator : ISqlGenerator
    {
        private readonly ISchemaService _schemaService;
        private readonly SqlPromptBuilder _promptBuilder;
        private readonly IGeminiClient _geminiClient;
        private readonly ISqlValidator _validator;

        public SqlGenerator(
            ISchemaService schemaService,
            SqlPromptBuilder promptBuilder,
            IGeminiClient geminiClient,
            ISqlValidator validator)
        {
            _schemaService = schemaService;
            _promptBuilder = promptBuilder;
            _geminiClient = geminiClient;
            _validator = validator; 
        }

        public async Task<string> GenerateSqlAsync(string question)
        {
            var schema = await _schemaService.GetDatabaseSchemaAsync();

            var prompt = _promptBuilder.Build(question, schema);

            var sql = (await _geminiClient.GenerateContentAsync(prompt)).Trim();

            Console.WriteLine("==================================");
            Console.WriteLine("AI GENERATED SQL");
            Console.WriteLine("==================================");
            Console.WriteLine(sql);
            Console.WriteLine("==================================");

            if (!_validator.IsSafe(sql))
            {
                throw new Exception("Unsafe SQL generated.");
            }

            return sql;
        }
    }
}
