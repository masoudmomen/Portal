using System.ComponentModel.DataAnnotations;

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

        // فعلاً به صورت String ذخیره می‌کنیم تا فرم شما کار کند
        public string ProjectManagerName { get; set; } = string.Empty;
        public string EngineerName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; } = DateTime.Now.AddMonths(3);
        public int Progress { get; set; }

        public string Status { get; set; } = "In Progress";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
