using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;

namespace Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تنظیمات اضافی برای اینتیتی پروژه
            builder.Entity<ProjectEntity>(entity =>
            {
                entity.HasIndex(e => e.ProjectCode).IsUnique(); // جلوگیری از کد تکراری
            });
        }
    }
}
