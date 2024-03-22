using System.Runtime.Serialization;

namespace K9_Koinz.Models.Helpers {
    [DataContract]
    public class AutocompleteResult {
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "val")]
        public Guid Id { get; set; }
    }
}
