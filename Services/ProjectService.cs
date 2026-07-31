using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Entities;

namespace Portal.Services
{
    public interface IProjectService
    {
        Task<List<ProjectEntity>> GetAllProjectsAsync(string? searchTerm = null, string? type = null, string? status = null);
        Task<ProjectEntity?> GetProjectByIdAsync(int id);
        Task<bool> CreateProjectAsync(ProjectEntity project);
        Task<bool> UpdateProjectAsync(ProjectEntity project);
        Task<bool> DeleteProjectAsync(int id);
    }

    public class ProjectService : IProjectService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ProjectService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ProjectEntity>> GetAllProjectsAsync(string? searchTerm = null, string? type = null, string? status = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.Trim();
                query = query.Where(p => p.Name.Contains(normalizedSearchTerm) 
                || p.ProjectCode.Contains(normalizedSearchTerm)
                || (p.ProjectManagerName != null &&
                    p.ProjectManagerName.Contains(normalizedSearchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.Type == type);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<ProjectEntity?> GetProjectByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Projects.FindAsync(id);
        }

        public async Task<bool> CreateProjectAsync(ProjectEntity project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            project.CreatedAt = DateTime.UtcNow;
            context.Projects.Add(project);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProjectAsync(ProjectEntity project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            project.UpdatedAt = DateTime.UtcNow;
            context.Entry(project).State = EntityState.Modified;
            context.Entry(project).Property(x => x.CreatedAt).IsModified = false;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.FindAsync(id);
            if (project == null) return false;

            context.Projects.Remove(project);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
