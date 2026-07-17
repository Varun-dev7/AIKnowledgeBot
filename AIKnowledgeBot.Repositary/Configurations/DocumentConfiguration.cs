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
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.OriginalFileName)
                   .HasMaxLength(255);

            builder.Property(x => x.StoredFileName)
                   .HasMaxLength(255);

            builder.Property(x => x.Extension)
                   .HasMaxLength(20);

            builder.Property(x => x.ContentType)
                   .HasMaxLength(100);

            builder.Property(x => x.FilePath)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Category)
                   .WithMany(x => x.Documents)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Subject)
                   .WithMany(x => x.Documents)
                   .HasForeignKey(x => x.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
