using System.ComponentModel.DataAnnotations;

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

        public enum ProjectType
        {
            MEP,
            Civil,
            Electrical,
        }

        public enum ProjectStatus
        {
            InProgress,
            Completed,
            OnHold,
        }
    }
}
