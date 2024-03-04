using System.ComponentModel;

namespace K9_Koinz.Models {
    public class RecurringTransfer : Transfer {
        public Guid RepeatConfigId { get; set; }
        [DisplayName("Repeat Settings")]
        public RepeatConfig RepeatConfig { get; set; }
    }
}
