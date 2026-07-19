using Portal.Models.Enums;

namespace Portal.Models
{
    public class TaskItemModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public EnumsClass.TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int Progress { get; set; }
        public EnumsClass.ActionPriority Priority { get; set; }
        public List<SubTaskItemModel> Subtasks { get; set; } = new();
    }
}
