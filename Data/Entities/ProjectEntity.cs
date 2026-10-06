using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portal.Data;
using Portal.Models.Enums;

namespace Portal.Data.Entities
{
    public class ProjectEntity
    {
        [Key]
        public int Id { get; set; } // PK دیتابیس

        [Required]
        [MaxLength(50)]
        public string ProjectCode { get; set; } = string.Empty; // همان ProjectID فرم

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "MEP";

        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public string ProjectManagerName { get; set; } = string.Empty;
        public string? ProjectManagerUserId { get; set; }
        [ForeignKey(nameof(ProjectManagerUserId))]
        public ApplicationUser? ProjectManagerUser { get; set; }

        public DateTime DueDate { get; set; } = DateTime.Now.AddMonths(3);
        public int Progress { get; set; }
        public EnumsClass.ProjectStatus Status { get; set; } = EnumsClass.ProjectStatus.New;
        public bool IsWon { get; set;  } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<TaskEntity> Tasks { get; set; } = new();
        public List<ProjectEngineerAssignment> EngineerAssignments { get; set; } = new();
    }
}
