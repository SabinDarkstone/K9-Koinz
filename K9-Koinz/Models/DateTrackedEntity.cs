namespace K9_Koinz.Models {
    public abstract class DateTrackedEntity : BaseEntity {
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
