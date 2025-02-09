namespace K9_Koinz.Models.Helpers {
    public enum SaveStatus {
        SUCCESS,
        ERROR
    }

    public class DbSaveResult {
        public bool IsSuccess => Status == SaveStatus.SUCCESS;
        public TriggerStatus BeforeStatus { get; set; }
        public TriggerStatus AfterStatus { get; set; }
        public SaveStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public List<Guid> Ids { get; set; } = new List<Guid>();
    }
}
