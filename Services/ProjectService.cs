using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Entities;
using Portal.Models;

namespace Portal.Services
{
    public interface IProjectService
    {
        Task<List<ProjectEntity>> GetAllProjectsAsync(string? searchTerm = null, string? type = null, string? status = null);
        Task<ProjectEntity?> GetProjectByIdAsync(int id);
        Task<List<ProjectUserOption>> GetActiveUsersAsync();
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
            var query = context.Projects
                .Include(p => p.ProjectManagerUser)
                .Include(p => p.EngineerAssignments)
                .ThenInclude(a => a.EngineerUser)
                .AsQueryable();

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

            return await query.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<ProjectEntity?> GetProjectByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Projects
                .Include(p => p.ProjectManagerUser)
                .Include(p => p.EngineerAssignments)
                .ThenInclude(a => a.EngineerUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProjectUserOption>> GetActiveUsersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .Where(user => user.IsActive)
                .OrderBy(user => user.FullName ?? user.UserName)
                .Select(user => new ProjectUserOption
                {
                    Id = user.Id,
                    DisplayName = string.IsNullOrWhiteSpace(user.FullName)
                        ? user.UserName ?? user.Email ?? user.Id
                        : user.FullName
                })
                .ToListAsync();
        }

        public async Task<bool> CreateProjectAsync(ProjectEntity project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            project.CreatedAt = DateTime.UtcNow;
            project.Progress = 0;
            project.Status = string.IsNullOrWhiteSpace(project.Status) ? "New" : project.Status;

            await ApplyUserAssignmentsAsync(context, project);
            context.Projects.Add(project);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProjectAsync(ProjectEntity project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            project.UpdatedAt = DateTime.UtcNow;
            project.Progress = await CalculateProjectProgressAsync(context, project.Id);
            await ApplyUserAssignmentsAsync(context, project);
            context.Entry(project).State = EntityState.Modified;
            context.Entry(project).Property(x => x.CreatedAt).IsModified = false;
            context.ProjectEngineerAssignments.AddRange(project.EngineerAssignments);
            return await context.SaveChangesAsync() > 0;
        }

        private static async Task ApplyUserAssignmentsAsync(ApplicationDbContext context, ProjectEntity project)
        {
            if (!string.IsNullOrWhiteSpace(project.ProjectManagerUserId))
            {
                var manager = await context.Users.FindAsync(project.ProjectManagerUserId);
                if (manager is not null)
                {
                    project.ProjectManagerName = GetUserDisplayName(manager);
                }
            }
            else
            {
                project.ProjectManagerName = string.Empty;
            }

            var engineerIds = project.EngineerAssignments
                .Select(assignment => assignment.EngineerUserId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var engineers = await context.Users
                .Where(user => engineerIds.Contains(user.Id))
                .ToListAsync();

            project.EngineerName = string.Join(", ", engineers.Select(GetUserDisplayName));
            project.EngineerAssignments = engineerIds
                .Select(id => new ProjectEngineerAssignment { ProjectId = project.Id, EngineerUserId = id })
                .ToList();

            if (project.Id != 0)
            {
                var existingAssignments = await context.ProjectEngineerAssignments
                    .Where(assignment => assignment.ProjectId == project.Id)
                    .ToListAsync();

                context.ProjectEngineerAssignments.RemoveRange(existingAssignments);
            }
        }

        private static async Task<int> CalculateProjectProgressAsync(ApplicationDbContext context, int projectId)
        {
            var progresses = await context.Tasks
                .Where(task => task.Action.ProjectId == projectId)
                .Select(task => task.Progress)
                .ToListAsync();

            return progresses.Count == 0 ? 0 : (int)Math.Round(progresses.Average());
        }

        private static string GetUserDisplayName(ApplicationUser user)
        {
            return string.IsNullOrWhiteSpace(user.FullName)
                ? user.UserName ?? user.Email ?? user.Id
                : user.FullName;
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
