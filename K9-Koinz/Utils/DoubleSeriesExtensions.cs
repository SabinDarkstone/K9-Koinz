using K9_Koinz.Models.Helpers;

namespace K9_Koinz.Utils {

    public static class DoubleSeriesUtils {
        public static IEnumerable<SeriesLine> Accumulate(this IEnumerable<SeriesLine> points) {
            double sum = 0;
            foreach (var point in points) {
                sum += point.Y;
                yield return new SeriesLine(point.X, Math.Floor(sum));
            }
        }

        public static List<SeriesLine> FillInGaps(this List<SeriesLine> points, DateTime date, bool doFullMonth) {
            var newList = new List<SeriesLine>();
            var max = date.Day;
            if (doFullMonth) {
                max = DateTime.DaysInMonth(date.Year, date.Month);
            }
            for (var i = 1; i < max + 1; i++) {
                var index = i - 1;
                if (points.Any(p => p.X == i)) {
                    newList.Add(points.First(p => p.X == i));
                } else {
                    if (i == 1) {
                        newList.Add(new SeriesLine(1d, 0));
                    } else {
                        newList.Add(new SeriesLine(i, newList[index - 1].Y));
                    }
                }
            }

            return newList;
        }
    }
}