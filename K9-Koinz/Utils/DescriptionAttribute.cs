namespace K9_Koinz.Utils {
    [AttributeUsage(AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute {
        private string Description;
        public double Version;

        public DescriptionAttribute(string description) {
            Description = description;
            Version = 1.0;
        }

        public string GetDescription() => Description;
    }

    // TODO
    public static class AttributeExtensions {
    }
}
