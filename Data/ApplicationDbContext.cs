using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // در ادامه اینجا DbSetهای پروژه را هم اضافه می‌کنیم
        // public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
        // public DbSet<ActionEntity> Actions => Set<ActionEntity>();
        // public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
        // public DbSet<ReportEntity> Reports => Set<ReportEntity>();
    }
}
