using Humanizer;
using K9_Koinz.Models;

namespace K9_Koinz.Utils {
    public static class RepeatConfigExtensions {

        private const string EXPIRED_MSG = "Never - Expired";
        private const string BAD_MODE_ERR = "Unknown repeat mode chosen";
        private const string BAD_FREQUENCY_ERR = "Unknown repeat frequency chosen.";

        public static string GetRepeatString(this RepeatConfig rptCfg) {
            if (!rptCfg.IsActive) {
                return EXPIRED_MSG;
            }

            switch (rptCfg.Mode) {
                case RepeatMode.SPECIFIC_DAY:
                    return GetStringForSpecificDay(rptCfg);
                case RepeatMode.INTERVAL:
                    return GetStringForInterval(rptCfg);
                default:
                    throw new Exception(BAD_MODE_ERR);
            }
        }

        private static string GetStringForSpecificDay(RepeatConfig rptCfg) {
            var nextFireDate = rptCfg.CalculatedNextFiring.Value;

            switch (rptCfg.Frequency) {
                case RepeatFrequency.DAILY:
                    return "Every Day";
                case RepeatFrequency.WEEKLY:
                    return $"Every Week on the {nextFireDate.DayOfWeek}";
                case RepeatFrequency.MONTHLY:
                    return $"Every Month on the {nextFireDate.Day.Ordinalize()}";
                case RepeatFrequency.YEARLY:
                    return $"Every Year on {nextFireDate.Month}/{nextFireDate.Day}";
                default:
                    throw new Exception(BAD_FREQUENCY_ERR);
            }
        }

        private static string GetStringForInterval(RepeatConfig rptCfg) {
            string intervalPeriod;
            switch (rptCfg.Frequency) {
                case RepeatFrequency.DAILY:
                    intervalPeriod = "Day";
                    break;
                case RepeatFrequency.WEEKLY:
                    intervalPeriod = "Week";
                    break;
                case RepeatFrequency.MONTHLY:
                    intervalPeriod = "Month";
                    break;
                case RepeatFrequency.YEARLY:
                    intervalPeriod = "Year";
                    break;
                default:
                    throw new Exception(BAD_FREQUENCY_ERR);
            }

            if (rptCfg.IntervalGap == 1) {
                return $"Every {intervalPeriod}";
            } else {
                return $"Every {rptCfg.IntervalGap} {intervalPeriod}s";
            }
        }
    }
}
