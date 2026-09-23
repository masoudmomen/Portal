using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Data.Entities;

public class ProjectEngineerAssignment
{
    [Required]
    public int ProjectId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public ProjectEntity Project { get; set; } = null!;

    [Required]
    public string EngineerUserId { get; set; } = string.Empty;

    [ForeignKey(nameof(EngineerUserId))]
    public ApplicationUser EngineerUser { get; set; } = null!;
}
