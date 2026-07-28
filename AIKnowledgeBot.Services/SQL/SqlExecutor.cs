using AIKnowledgeBot.InterFace.IService;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace AIKnowledgeBot.Services.SQL
{
    public class SqlExecutor : ISqlExecutor
    {
        private readonly IConfiguration _configuration;

        public SqlExecutor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DataTable> ExecuteAsync(string sql)
        {
            var table = new DataTable();

            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            await using var reader =
                await command.ExecuteReaderAsync();

            table.Load(reader);

            return table;
        }
    }
}
