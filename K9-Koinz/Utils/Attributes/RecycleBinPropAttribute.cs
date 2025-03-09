namespace K9_Koinz.Utils.Attributes {

    [AttributeUsage(AttributeTargets.Property)]
    public class RecycleBinPropAttribute : Attribute {
        public string DisplayName { get; }

        public RecycleBinPropAttribute(string displayName) => DisplayName = displayName;
    }
}
