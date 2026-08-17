using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data.Entities
{
    public class SubTaskEntity
    {
        public int Id { get; set; }
        [Required]
        public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public TaskEntity Task { get; set; } = null!;
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
    }
}
