using K9_Koinz.Utils.Attributes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace K9_Koinz.Models.Meta {
    public abstract class BaseEntity {
        public Guid Id { get; set; }

        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Last Modified Date")]
        [RecycleBinProp("Last Modified Date")]
        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return GetType().ToString() + ": " + ToJson();
        }

        public virtual string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
