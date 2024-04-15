using Newtonsoft.Json;

namespace K9_Koinz.Utils {
    public static class JsonUtils {
        public static string ToLogJson(this object obj) {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            return json;
        }
    }
}
