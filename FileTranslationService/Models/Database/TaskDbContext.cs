using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public class TaskDbContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<TranslationStatus> TranslationStatuses { get; set; }
        public DbSet<FileCategory> FileCategories { get; set; }

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Seed database

            foreach (var status in (TranslationStatusType[])Enum.GetValues(typeof(TranslationStatusType)))
            {
                modelBuilder.Entity<TranslationStatus>().HasData(
                    new TranslationStatus
                    {
                        Id = (int)status,
                        Description = status.ToString()
                    }
                );
            }

            foreach (var status in (Enums.FileCategory[])Enum.GetValues(typeof(Enums.FileCategory)))
            {
                modelBuilder.Entity<FileCategory>().HasData(
                    new FileCategory
                    {
                        Id = (int)status,
                        Description = status.ToString()
                    }
                );
            }

            #endregion
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IDbEntityTimestamps && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((IDbEntityTimestamps)entity.Entity).DbCreatedAt = now;
                }
                ((IDbEntityTimestamps)entity.Entity).DbUpdatedAt = now;
            }
        }
    }
}
