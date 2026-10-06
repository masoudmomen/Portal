using Portal.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data.Entities
{
    public class TaskEntity
    {
        public int Id { get; set; }
        [Required]
        public string? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public ProjectEntity Project { get; set; } = null!;
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string AssignedById { get; set; }

        [ForeignKey("AssignedById")]
        public virtual ApplicationUser AssignedBy { get; set; }

        public EnumsClass.TaskStatus Status { get; set; } = EnumsClass.TaskStatus.New;
        public EnumsClass.TaskPriority Priority { get; set; } = EnumsClass.TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        [Range(0, 100)]
        public int Progress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<SubTaskEntity> Subtasks { get; set; } = new();
        public List<TaskUserAssignment> UserAssignments { get; set; } = new();
    }
}
