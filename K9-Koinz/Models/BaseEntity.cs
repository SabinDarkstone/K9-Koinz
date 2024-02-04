namespace K9_Koinz.Models {
    public abstract class BaseEntity {
        public Guid Id { get; set; }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
