using K9_Koinz.Pages;

namespace K9_Koinz.Utils {
    public static class ChartUtils {

        public static IEnumerable<Point> Accumulate(this IEnumerable<Point> points) {
            double sum = 0;
            foreach (var point in points) {
                sum += point.Y;
                yield return new Point(point.X, Math.Floor(sum));
            }
        }

        public static List<Point> FillInGaps(this List<Point> points, DateTime date, bool doFullMonth) {
            var newList = new List<Point>();
            var max = date.Day;
            if (doFullMonth) {
                max = DateTime.DaysInMonth(date.Year, date.Month);
            }
            var index = 0;
            for (var i = 1; i < max + 1; i++) {
                if (points.Any(p => p.X == i)) {
                    newList.Add(points.Single(p => p.X == i));
                } else {
                    if (i == 1) {
                        newList.Add(new Point(1, 0));
                    } else {
                        newList.Add(new Point(i, newList[index - 1].Y));
                    }
                }
                index += 1;
            }

            return newList;
        }
    }
}
