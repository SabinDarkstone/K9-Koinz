using System.Runtime.Serialization;

namespace K9_Koinz.Models.Helpers {

    [DataContract]
    public class SeriesLine {
        [DataMember(Name = "x")]
        public double X { get; set; }

        [DataMember(Name = "y")]
        public double Y { get; set; }

        public SeriesLine(double x, double y) {
            X = x;
            Y = y;
        }
    }

    [DataContract]
    public class SeriesColumn {
        [DataMember(Name = "label")]
        public string X { get; set; }

        [DataMember(Name = "y")]
        public double Y { get; set; }

        public SeriesColumn(string x, double y) {
            X = x;
            Y = y;
        }
    }
}