using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class ScheduledJobStatus : BaseEntity {
        public string JobName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string StackTrace { get; set; }
        public string ErrorMessages { get; set; }
        public DateTime NextRunTime { get; set; }
    }
}
