using K9_Koinz.Models.Helpers;

namespace K9_Koinz.Utils {
    public static class ChartUtils {
        public static IEnumerable<GraphDataPoint> Accumulate(this IEnumerable<GraphDataPoint> points) {
            double sum = 0;
            foreach (var point in points) {
                sum += point.Y.Value;
                yield return new GraphDataPoint(point.Label, Math.Floor(sum));
            }
        }

        public static List<GraphDataPoint> FillInGaps(this List<GraphDataPoint> points, DateTime date, bool doFullMonth) {
            var newList = new List<GraphDataPoint>();
            var max = date.Day;
            if (doFullMonth) {
                max = DateTime.DaysInMonth(date.Year, date.Month);
            }
            for (var i = 1; i < max + 1; i++) {
                var index = i - 1;
                if (points.Any(p => p.Label == i.ToString())) {
                    newList.Add(points.First(p => p.Label == i.ToString()));
                } else {
                    if (i == 1) {
                        newList.Add(new GraphDataPoint(1.ToString(), 0));
                    } else {
                        newList.Add(new GraphDataPoint(i.ToString(), newList[index - 1].Y));
                    }
                }
            }

            return newList;
        }
    }
}
