using Newtonsoft.Json;

namespace K9_Koinz.Utils {
    public static class JsonUtils {
        public static JsonSerializerSettings DefaultSettings => new JsonSerializerSettings {
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.None
        };
    }
}