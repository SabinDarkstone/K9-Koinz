using System.Runtime.Serialization;

namespace K9_Koinz.Models.Helpers {

    [DataContract]
    public class GraphDataPoint {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "y")]
        public double? Y { get; set; }
    }
}
