using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Repositary.Context;
using AIKnowledgeBot.Repositary.Repository;
using AIKnowledgeBot.Repositary.UnitOfWork;
using AIKnowledgeBot.Services.AI;
using AIKnowledgeBot.Services.Chat;
using AIKnowledgeBot.Services.Chunking;
using AIKnowledgeBot.Services.Common;
using AIKnowledgeBot.Services.Document;
using AIKnowledgeBot.Services.Embedding;
using AIKnowledgeBot.Services.Extractors;
using AIKnowledgeBot.Services.Hybrid;
using AIKnowledgeBot.Services.Pipeline;
using AIKnowledgeBot.Services.Search;
using AIKnowledgeBot.Services.SQL;
using Microsoft.EntityFrameworkCore;
namespace AIKnowledgeBot.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ISubjectRepository, SubjectRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDocumentTextExtractor, PdfTextExtractor>();

            services.AddScoped<IDocumentTextExtractor, DocxTextExtractor>();

            services.AddScoped<IDocumentTextExtractor, TxtTextExtractor>();

            services.AddScoped<IDocumentProcessorService, DocumentProcessorService>();
            services.AddScoped<IChunkService, ChunkService>();
            services.AddScoped<IKnowledgePipelineService, KnowledgePipelineService>();
            services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
            services.AddScoped<IFilePathProvider, FilePathProvider>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<ISimilarityService, SimilarityService>();
            services.AddScoped<ISemanticSearchService, SemanticSearchService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IDocumentEmbeddingRepository, DocumentEmbeddingRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();

            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IIntentDetectionService, IntentDetectionService>();
            services.AddScoped<ISqlService, SqlService>();
            services.AddScoped<ISchemaService, SchemaService>();
            services.AddScoped<SqlPromptBuilder>();
            services.AddScoped<ISqlGenerator, SqlGenerator>();
            services.AddScoped<ISqlValidator, SqlValidator>();
            services.AddScoped<ISqlExecutor, SqlExecutor>();
            services.AddScoped<IResultSummarizer, ResultSummarizer>();
            services.AddScoped<ISqlService, SqlService>();
            services.AddScoped<IHybridSearchService, HybridSearchService>();
            return services;
        }
    }
}
