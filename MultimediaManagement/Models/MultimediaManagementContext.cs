using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MultimediaManagement.Models
{
    public partial class MultimediaManagementContext : DbContext
    {
        public MultimediaManagementContext()
        {
        }

        public MultimediaManagementContext(DbContextOptions<MultimediaManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Collection> Collection { get; set; }
        public virtual DbSet<EntityFile> EntityFile { get; set; }
        public virtual DbSet<Placeholder> Placeholder { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=MultimediaManagement;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Collection)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Collection_User");
            });

            modelBuilder.Entity<EntityFile>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Data).HasColumnType("ntext");

                entity.Property(e => e.Extension)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.IsUrl).HasColumnName("IsURL");

                entity.Property(e => e.PlaceholderId).HasColumnName("PlaceholderID");

                entity.HasOne(d => d.Placeholder)
                    .WithMany(p => p.EntityFile)
                    .HasForeignKey(d => d.PlaceholderId)
                    .HasConstraintName("FK_EntityFile_Placeholder");
            });

            modelBuilder.Entity<Placeholder>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CollectionId).HasColumnName("CollectionID");

                entity.Property(e => e.Data).HasColumnType("ntext");

                entity.Property(e => e.Extension).HasMaxLength(30);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Placeholder)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("FK_Placeholder_Collection");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(60);
            });
        }
    }
}
