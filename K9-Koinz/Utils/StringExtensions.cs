namespace K9_Koinz.Utils {
    public static class StringExtensions {
        public static Guid ToGuid(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return Guid.Empty;
            } else {
                return Guid.Parse(value);
            }
        }
    }
}
