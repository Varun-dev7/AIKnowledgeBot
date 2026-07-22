using AIKnowledgeBot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Document = AIKnowledgeBot.Models.Entities.Document;

namespace AIKnowledgeBot.Repositary.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Subject> Subjects => Set<Subject>();

        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
        public DbSet<DocumentEmbedding> DocumentEmbeddings => Set<DocumentEmbedding>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Conversation>().HasMany(x => x.Messages).WithOne(x => x.Conversation)
                        .HasForeignKey(x => x.ConversationId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ChatMessage>().Property(x => x.Role).HasConversion<string>();
        }

    }
}
