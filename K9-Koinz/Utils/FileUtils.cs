using K9_Koinz.Pages;
using System.Text;

namespace K9_Koinz.Utils {
    public static class FileUtils {
        public static List<string> ReadAsList(this IFormFile file) {
            var result = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream())) {
                while (reader.Peek() >= 0) {
                    result.Add(reader.ReadLine());
                }
            }

            return result;
        }
    }
}
