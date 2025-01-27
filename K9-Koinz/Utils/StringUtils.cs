namespace K9_Koinz.Utils {
    public static class StringUtils {
        public static string Truncate(this string text, int maxChars = 25) {
            if (text == null) {
                return string.Empty;
            }
            return text.Length <= maxChars ? text.Trim() : text.Substring(0, maxChars).TrimEnd() + "...";
        }
    }
}
