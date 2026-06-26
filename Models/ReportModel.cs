namespace Portal.Models
{
    public class ReportModel
    {
        public string ReportId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reporter { get; set; } = "";
        public string ReporterId { get; set; } = "";
        public string SubmittedTo { get; set; } = "";
        public string Department { get; set; } = "Engineering";
        public string Category { get; set; } = "Daily";
        public DateTime WorkDate { get; set; } = DateTime.Today;
        public DateTime ReportDate { get; set; } = DateTime.Today;
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public bool IsRead { get; set; } = false;
    }

    public enum ReportStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum ReportFilterCategory
    {
        None,
        Unread,
        Today,
        ThisWeek,
        PendingReview
    }
}
