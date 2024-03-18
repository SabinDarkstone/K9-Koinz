using Newtonsoft.Json;

namespace K9_Koinz.Models.Meta {
    public abstract class BaseEntity {
        public Guid Id { get; set; }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return GetType().ToString() + ": " + ToJson();
        }

        public virtual string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { MaxDepth = 6 });
        }
    }
}
