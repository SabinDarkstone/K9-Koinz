using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class ErrorLog : BaseEntity {
        public string ExceptionString { get; set; }
        public string ClassName { get; set; }
        public string Message { get; set; }
        public string StackTraceString { get; set; }
        public string CurrentRoute { get; set; }

        public Guid? InnerExceptionId { get; set; }
        public ErrorLog InnerException { get; set; }
    }
}
