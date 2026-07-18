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
    public class DocumentChunkConfiguration
        : IEntityTypeConfiguration<DocumentChunk>
    {
        public void Configure(EntityTypeBuilder<DocumentChunk> builder)
        {
            builder.ToTable("DocumentChunks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.HasOne(x => x.Document)
                   .WithMany(x => x.Chunks)
                   .HasForeignKey(x => x.DocumentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
