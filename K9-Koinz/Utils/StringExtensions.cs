namespace K9_Koinz.Utils {
    public static class StringExtensions {
        public static Guid? ToGuid(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return null;
            } else {
                return Guid.Parse(value);
            }
        }
    }
}
