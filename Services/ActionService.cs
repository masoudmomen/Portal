using Microsoft.EntityFrameworkCore;
using Portal.Data;
using Portal.Data.Entities;
using Portal.Models;
using Portal.Models.Enums;

namespace Portal.Services
{

    public interface IActionService
    {
        Task<List<ActionItemModel>> GetAllActionsAsync();
        Task<ActionItemModel?> GetActionByIdAsync(int id);

        Task<int> CreateActionAsync(ActionFormModel model);
        Task<bool> UpdateActionAsync(ActionFormModel model);
        Task<bool> DeleteActionAsync(int id);

        // متد جدید برای دریافت لیست پروژه‌ها جهت دراپ‌دان‌ها
        Task<List<ProjectLookupModel>> GetProjectsLookupAsync();

        Task<TaskItemModel?> CreateTaskAsync(int actionId, TaskItemModel taskModel);
        Task<bool> UpdateTaskAsync(TaskItemModel taskModel);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<SubTaskItemModel?> CreateSubtaskAsync(int taskId, string title, string description = "");
        Task<bool> ToggleSubtaskAsync(int subtaskId, bool isDone);
        Task<bool> DeleteSubtaskAsync(int subtaskId);
        Task<bool> UpdateSubtaskAsync(int subtaskId, string title, string description);
    }
    public class ActionService : IActionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ActionService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ActionItemModel>> GetAllActionsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var actions = await context.Actions
                .Include(a => a.Project)
                .Include(a => a.Tasks)
                    .ThenInclude(t => t.Subtasks)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return actions.Select(MapToActionItemModel).ToList();
        }

        public async Task<ActionItemModel?> GetActionByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var action = await context.Actions
                .Include(a => a.Project)
                .Include(a => a.Tasks)
                    .ThenInclude(t => t.Subtasks)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (action == null)
                return null;

            return MapToActionItemModel(action);
        }

        public async Task<int> CreateActionAsync(ActionFormModel model)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = new ActionEntity
            {
                ProjectId = model.ProjectId,
                Title = model.Title?.Trim() ?? string.Empty,
                Description = model.Description?.Trim() ?? string.Empty,
                AssignedTo = model.AssignedTo?.Trim() ?? string.Empty,
                AssignedBy = model.AssignedBy?.Trim() ?? string.Empty,
                Status = model.Status,
                Priority = model.Priority,
                DueDate = model.DueDateText,
                Progress = CalculateActionProgress(model.Status),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            context.Actions.Add(entity);
            await context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateActionAsync(ActionFormModel model)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.Actions.FirstOrDefaultAsync(a => a.Id == model.Id);
            if (entity == null)
                return false;

            entity.ProjectId = model.ProjectId;
            entity.Title = model.Title?.Trim() ?? string.Empty;
            entity.Description = model.Description?.Trim() ?? string.Empty;
            entity.AssignedTo = model.AssignedTo?.Trim() ?? string.Empty;
            entity.AssignedBy = model.AssignedBy?.Trim() ?? string.Empty;
            entity.Status = model.Status;
            entity.Priority = model.Priority;
            entity.DueDate = model.DueDateText;
            entity.Progress = CalculateActionProgress(model.Status);
            entity.UpdatedAt = DateTime.Now;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteActionAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.Actions.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
                return false;

            context.Actions.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }

        private static ActionItemModel MapToActionItemModel(ActionEntity entity)
        {
            return new ActionItemModel
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                ProjectName = entity.Project?.Name ?? string.Empty,
                Title = entity.Title,
                Description = entity.Description,
                AssignedTo = entity.AssignedTo,
                AssignedBy = entity.AssignedBy,
                Status = entity.Status,
                Priority = entity.Priority,
                DueDate = entity.DueDate,
                Progress = entity.Progress,
                Tasks = entity.Tasks?
                    .OrderBy(t => t.Id)
                    .Select(t => MapToTaskItemModel(t, entity.ProjectId, entity.Project?.Name))
                    .ToList() ?? new List<TaskItemModel>()
            };
        }

        private static TaskItemModel MapToTaskItemModel(TaskEntity entity, int? projectId, string? projectName)
        {
            return new TaskItemModel
            {
                Id = entity.Id,
                ProjectId = projectId,
                ProjectName = projectName ?? string.Empty,
                Title = entity.Title,
                Description = entity.Description,
                AssignedTo = entity.AssignedTo,
                Status = entity.Status,
                Priority = entity.Priority,
                DueDate = entity.DueDate,
                Progress = entity.Progress,
                Subtasks = entity.Subtasks?
                    .OrderBy(st => st.OrderIndex)
                    .Select(MapToSubTaskItemModel)
                    .ToList() ?? new List<SubTaskItemModel>()
            };
        }

        private static SubTaskItemModel MapToSubTaskItemModel(SubTaskEntity entity)
        {
            return new SubTaskItemModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                IsDone = entity.IsDone,
                OrderIndex = entity.OrderIndex
            };
        }

        private static int CalculateActionProgress(EnumsClass.ActionStatus status)
        {
            return status switch
            {
                EnumsClass.ActionStatus.New => 0,
                EnumsClass.ActionStatus.Assigned => 10,
                EnumsClass.ActionStatus.InProgress => 50,
                EnumsClass.ActionStatus.Blocked => 50,
                EnumsClass.ActionStatus.Completed => 100,
                EnumsClass.ActionStatus.Canceled => 0,
                _ => 0
            };
        }

        public async Task<List<ProjectLookupModel>> GetProjectsLookupAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Projects
                .AsNoTracking()
                .Select(p => new ProjectLookupModel
                {
                    Id = p.Id,
                    Title = p.Name
                })
                .ToListAsync();
        }

        // ==========================================
        // متدهای مدیریت Tasks
        // ==========================================

        public async Task<TaskItemModel?> CreateTaskAsync(int actionId, TaskItemModel taskModel)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var action = await context.Actions
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.Id == actionId);

            if (action == null) return null;

            var taskEntity = new TaskEntity
            {
                ActionId = actionId,
                Title = taskModel.Title,
                Description = taskModel.Description,
                AssignedTo = string.IsNullOrWhiteSpace(taskModel.AssignedTo) ? action.AssignedTo : taskModel.AssignedTo,
                AssignedBy = action.AssignedBy,
                Status = taskModel.Status,
                Priority = taskModel.Priority,
                Progress = taskModel.Progress,
                DueDate = taskModel.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            context.Tasks.Add(taskEntity);
            await context.SaveChangesAsync();

            taskModel.Id = taskEntity.Id;
            taskModel.ProjectId = action.ProjectId;
            taskModel.ProjectName = action.Project?.Name ?? string.Empty;
            taskModel.Subtasks = new List<SubTaskItemModel>();

            return taskModel;
        }

        public async Task<bool> UpdateTaskAsync(TaskItemModel taskModel)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var task = await context.Tasks.FindAsync(taskModel.Id);
            if (task == null) return false;

            task.Title = taskModel.Title;
            task.Description = taskModel.Description;
            task.AssignedTo = taskModel.AssignedTo;
            task.Status = taskModel.Status;
            task.Priority = taskModel.Priority;
            task.Progress = taskModel.Progress;
            task.DueDate = taskModel.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var task = await context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            return true;
        }

        // ==========================================
        // متدهای مدیریت SubTasks
        // ==========================================

        public async Task<SubTaskItemModel?> CreateSubtaskAsync(int taskId, string title, string description = "")
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var task = await context.Tasks
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return null;

            var nextOrder = task.Subtasks.Any() ? task.Subtasks.Max(s => s.OrderIndex) + 1 : 1;

            var subtaskEntity = new SubTaskEntity
            {
                TaskId = taskId,
                Title = title,
                Description = description,
                IsDone = false,
                OrderIndex = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            context.SubTasks.Add(subtaskEntity);
            await context.SaveChangesAsync();

            return new SubTaskItemModel
            {
                Id = subtaskEntity.Id,
                Title = subtaskEntity.Title,
                Description = subtaskEntity.Description,
                IsDone = subtaskEntity.IsDone,
                OrderIndex = subtaskEntity.OrderIndex
            };
        }

        public async Task<bool> ToggleSubtaskAsync(int subtaskId, bool isDone)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var subtask = await context.SubTasks.FindAsync(subtaskId);
            if (subtask == null) return false;

            subtask.IsDone = isDone;
            subtask.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            // به‌روزرسانی خودکار درصد پیشرفت و وضعیت Task والد در دیتابیس
            var parentTask = await context.Tasks
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == subtask.TaskId);

            if (parentTask != null && parentTask.Subtasks.Any())
            {
                var total = parentTask.Subtasks.Count;
                var doneCount = parentTask.Subtasks.Count(s => s.IsDone);
                var progress = (int)Math.Round((double)doneCount / total * 100);

                parentTask.Progress = progress;

                if (progress == 100)
                    parentTask.Status = EnumsClass.TaskStatus.Completed;
                else if (progress > 0 && parentTask.Status == EnumsClass.TaskStatus.New)
                    parentTask.Status = EnumsClass.TaskStatus.InProgress;

                parentTask.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteSubtaskAsync(int subtaskId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var subtask = await context.SubTasks.FindAsync(subtaskId);
            if (subtask == null) return false;

            var parentTaskId = subtask.TaskId;
            context.SubTasks.Remove(subtask);
            await context.SaveChangesAsync();

            // محاسبه مجدد درصد پیشرفت Task والد پس از حذف Subtask
            var parentTask = await context.Tasks
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == parentTaskId);

            if (parentTask != null)
            {
                if (parentTask.Subtasks.Any())
                {
                    var total = parentTask.Subtasks.Count;
                    var doneCount = parentTask.Subtasks.Count(s => s.IsDone);
                    parentTask.Progress = (int)Math.Round((double)doneCount / total * 100);
                }
                else
                {
                    parentTask.Progress = 0;
                }

                parentTask.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> UpdateSubtaskAsync(int subtaskId, string title, string description)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var subtask = await context.SubTasks.FindAsync(subtaskId);
            if (subtask == null) return false;

            subtask.Title = title;
            subtask.Description = description;
            subtask.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

    }
}
