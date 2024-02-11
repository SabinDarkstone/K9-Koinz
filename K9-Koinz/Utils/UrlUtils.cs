using System.Text;
using System.Text.Json;

namespace K9_Koinz.Utils {
    public static class UrlUtils {

        private static readonly char[] _padding = { '=' };

        public static string Base64Encode(object inputObject) {
            var jsonString = JsonSerializer.Serialize(inputObject);
            var jsonAsBytes = Encoding.ASCII.GetBytes(jsonString);
            return Convert.ToBase64String(jsonAsBytes)
                .TrimEnd(_padding)
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static T Base64Decode<T>(string inputString) {
            var jsonString = inputString.Replace('_', '/').Replace('-', '+');
            switch (jsonString.Length % 4) {
                case 2:
                    jsonString += "==";
                    break;
                case 3:
                    jsonString += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(jsonString);
            return JsonSerializer.Deserialize<T>(bytes);
        }
    }
}
