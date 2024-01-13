using System.ComponentModel;

namespace K9_Koinz.Models {
    public abstract class DateTrackedEntity : BaseEntity {
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime LastModifiedDate { get; set; }
    }
}
