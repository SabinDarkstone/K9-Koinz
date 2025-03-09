using K9_Koinz.Models.Meta;
using K9_Koinz.Utils.Attributes;

namespace K9_Koinz.Models {
    public class ErrorLog : BaseEntity {
        public string ExceptionString { get; set; }
        [RecycleBinProp("Class")]
        public string ClassName { get; set; }
        [RecycleBinProp("Error Message")]
        public string Message { get; set; }
        public string StackTraceString { get; set; }
        public string CurrentRoute { get; set; }

        public Guid? InnerExceptionId { get; set; }
        public ErrorLog InnerException { get; set; }
    }
}
