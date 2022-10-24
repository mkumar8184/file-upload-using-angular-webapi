using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AttachmentDomain.Entity
{
    public partial class AttachmentExampleContext : DbContext
    {
        public AttachmentExampleContext()
        {
        }

        public AttachmentExampleContext(DbContextOptions<AttachmentExampleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachments { get; set; } = null!;

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.Base64Url).IsUnicode(false);

                entity.Property(e => e.DocumentName).HasMaxLength(500);

                entity.Property(e => e.DocumentPath).HasMaxLength(1000);

                entity.Property(e => e.Extension).HasMaxLength(50);

                entity.Property(e => e.UploadedOn).HasColumnType("date");

                entity.Property(e => e.VirtualFileName).HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
