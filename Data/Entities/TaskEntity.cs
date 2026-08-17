using Portal.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data.Entities
{
    public class TaskEntity
    {
        public int Id { get; set; }
        [Required]
        public int ActionId { get; set; }
        [ForeignKey(nameof(ActionId))]
        public ActionEntity Action { get; set; } = null!;
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string AssignedTo { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string AssignedBy { get; set; } = string.Empty;
        public EnumsClass.TaskStatus Status { get; set; } = EnumsClass.TaskStatus.New;
        public EnumsClass.ActionPriority Priority { get; set; } = EnumsClass.ActionPriority.Medium;
        public DateTime? DueDate { get; set; }
        [Range(0, 100)]
        public int Progress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<SubTaskEntity> Subtasks { get; set; } = new();
    }
}
