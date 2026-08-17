using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using System.Reflection.Emit;

namespace Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
        public DbSet<ActionEntity> Actions => Set<ActionEntity>();
        public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
        public DbSet<SubTaskEntity> SubTasks => Set<SubTaskEntity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تنظیمات اضافی برای اینتیتی پروژه
            builder.Entity<ProjectEntity>(entity =>
            {
                entity.HasIndex(e => e.ProjectCode).IsUnique(); // جلوگیری از کد تکراری
            });


            builder.Entity<ActionEntity>(entity =>
            {
                entity.ToTable("Actions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(2000);

                entity.Property(x => x.AssignedTo)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.AssignedBy)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.Priority)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(x => x.ProjectId);
                entity.HasIndex(x => x.Status);
                entity.HasIndex(x => x.AssignedTo);
                entity.HasIndex(x => x.DueDate);

                entity.HasOne(x => x.Project)
                    .WithMany()
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<TaskEntity>(entity =>
            {
                entity.ToTable("Tasks");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                .HasMaxLength(2000);

                entity.Property(x => x.AssignedTo)
                .HasMaxLength(150);

                entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

                entity.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

                entity.HasIndex(x => x.ActionId);

                entity.HasOne(x => x.Action)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.ActionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SubTaskEntity>(entity =>
            {
                entity.ToTable("SubTasks");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(2000);

                entity.HasIndex(x => x.TaskId);
                entity.HasIndex(x => new { x.TaskId, x.OrderIndex });

                entity.HasOne(x => x.Task)
                    .WithMany(x => x.Subtasks)
                    .HasForeignKey(x => x.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
