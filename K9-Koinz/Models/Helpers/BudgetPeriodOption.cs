using K9_Koinz.Utils;

namespace K9_Koinz.Models.Helpers {
    public class BudgetPeriodOption {
        public DateTime Value { get; set; }
        public string ValueString {
            get {
                return Value.FormatForUrl();
            }
        }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
    }
}
