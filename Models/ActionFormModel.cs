using Portal.Models.Enums;

namespace Portal.Models
{
    public class ActionFormModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public EnumsClass.ActionStatus Status { get; set; }
        public EnumsClass.ActionPriority Priority { get; set; }
        public DateTime? DueDateText { get; set; }
    }
}
