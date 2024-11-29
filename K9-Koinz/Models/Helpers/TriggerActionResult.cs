namespace K9_Koinz.Models.Helpers {

    public enum TriggerStatus {
        NO_TRIGGER,
        SUCCESS,
        DUPLICATE_FOUND,
        GO_SAVINGS,
        ERROR
    }

    public class TriggerActionResult {
        public bool IsSuccess => Status != TriggerStatus.ERROR;
        public TriggerStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
