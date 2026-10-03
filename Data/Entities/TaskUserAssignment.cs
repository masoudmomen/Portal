using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data.Entities;

public class TaskUserAssignment
{
    [Required]
    public int TaskId { get; set; }

    [ForeignKey(nameof(TaskId))]
    public TaskEntity Task { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}
