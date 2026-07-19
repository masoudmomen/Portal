namespace Portal.Models.Enums
{
    public class EnumsClass
    {
        public enum ActionStatus
        {
            New,
            Assigned,
            InProgress,
            Blocked,
            Completed,
            Canceled
        }

        public enum TaskStatus
        {
            New,
            InProgress,
            Blocked,
            Completed
        }

        public enum ActionPriority
        {
            Low,
            Medium,
            High,
            Critical
        }
    }
}
