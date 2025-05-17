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

        public static List<DateOnly> GetNextSundays(DateOnly startDate, int count)
        {
            var sundays = new List<DateOnly>();

            // 先找出 startDate 之后的第一个周日（包含当天的话加上判断）
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)startDate.DayOfWeek + 7) % 7;
            var firstSunday = startDate.AddDays(daysUntilSunday == 0 ? 7 : daysUntilSunday); // 不包含 startDate 当天

            for (int i = 0; i < count; i++)
            {
                sundays.Add(firstSunday.AddDays(i * 7));
            }

            return sundays;
        }

    }
}
