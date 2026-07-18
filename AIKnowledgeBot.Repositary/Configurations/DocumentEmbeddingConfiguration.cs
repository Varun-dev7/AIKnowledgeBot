using AIKnowledgeBot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Repositary.Configurations
{
    public class DocumentEmbeddingConfiguration
        : IEntityTypeConfiguration<DocumentEmbedding>
    {
        public void Configure(EntityTypeBuilder<DocumentEmbedding> builder)
        {
            builder.ToTable("DocumentEmbeddings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmbeddingJson)
                   .IsRequired();

            builder.Property(x => x.Model)
                   .HasMaxLength(100);

            builder.HasOne(x => x.Chunk)
                   .WithOne(x => x.Embedding)
                   .HasForeignKey<DocumentEmbedding>(x => x.ChunkId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
