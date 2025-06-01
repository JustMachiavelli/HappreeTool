namespace HappreeTool.CommonUtils
{
    public class DateUtils
    {

        /// <summary>
        /// 计算两日期之间的天数差距
        /// </summary>
        /// <param name="date1">%Y-%m-%d...</param>
        /// <param name="date2">%Y-%m-%d...</param>
        /// <returns>绝对值天数</returns>
        public static int CalculateDaysDifference(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return (int)difference.TotalDays;
        }

        public static List<DateOnly> GetNextSundays(DateOnly startDate, int? count = null)
        {
            var sundays = new List<DateOnly>();

            if (count is not null)
            {
                // 找出 startDate 之后的第一个周日（不含 startDate 当天）
                int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)startDate.DayOfWeek + 7) % 7;
                var firstSunday = startDate.AddDays(daysUntilSunday == 0 ? 7 : daysUntilSunday);

                for (int i = 0; i < count; i++)
                {
                    sundays.Add(firstSunday.AddDays(i * 7));
                }
            }
            else
            {
                // 包含 startDate 和今天之间的所有周日
                var today = DateOnly.FromDateTime(DateTime.Today);
                var current = startDate;

                // 从 startDate 开始找每一个周日
                while (current <= today)
                {
                    if (current.DayOfWeek == DayOfWeek.Sunday)
                    {
                        sundays.Add(current);
                    }

                    current = current.AddDays(1);
                }
            }

            return sundays;
        }

        /// <summary>
        /// 获取离今天最近的一个星期日（包括今天）
        /// </summary>
        /// <returns>最近的一个星期日</returns>
        public static DateOnly GetLastOrTodaySunday()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int daysSinceSunday = ((int)today.DayOfWeek - (int)DayOfWeek.Sunday + 7) % 7;
            return today.AddDays(-daysSinceSunday);
        }

        /// <summary>
        /// 获取两个日期之间的日期范围
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DateOnly[] GetDateRange(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            int days = (endDate.DayNumber - startDate.DayNumber) + 1;
            var result = new DateOnly[days];

            for (int i = 0; i < days; i++)
            {
                result[i] = startDate.AddDays(i);
            }

            return result;
        }

    }
}
