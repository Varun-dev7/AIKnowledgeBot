using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IDocumentRepository Documents { get; }

        ICategoryRepository Categories { get; }

        ISubjectRepository Subjects { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
        IDocumentChunkRepository Chunks { get; }
        // THIS MUST EXIST
        IDocumentEmbeddingRepository Embeddings { get; }

        IConversationRepository Conversations { get; }

        IChatMessageRepository ChatMessages { get; }
    }
}
