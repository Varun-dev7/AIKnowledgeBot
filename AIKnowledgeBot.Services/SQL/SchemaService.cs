using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Repositary.Context;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AIKnowledgeBot.Services.SQL
{
    public class SchemaService : ISchemaService
    {
        private readonly ApplicationDbContext _context;

        public SchemaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<string> GetDatabaseSchemaAsync()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Database Schema");
            sb.AppendLine("========================");

            var entities = _context.Model.GetEntityTypes();

            foreach (var entity in entities)
            {
                var tableName = entity.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                    continue;

                sb.AppendLine();
                sb.AppendLine($"Table: {tableName}");
                sb.AppendLine("--------------------------------");

                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName(StoreObjectIdentifier.Table(tableName, null));

                    sb.AppendLine($"{columnName} ({property.ClrType.Name})");
                }
            }

            return Task.FromResult(sb.ToString());
        }
    }
}
