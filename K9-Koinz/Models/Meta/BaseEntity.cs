namespace K9_Koinz.Models.Meta {
    public abstract class BaseEntity {
        public Guid Id { get; set; }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
