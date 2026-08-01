namespace HappreeTool.Utils.CommonUtils
{
    public static class DateUtils
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
        /// 获取指定时间段内的所有星期日（左闭右闭）
        /// </summary>
        /// <param name="start">开始日期（包含）</param>
        /// <param name="end">结束日期（包含）</param>
        /// <returns>区间内的所有星期日</returns>
        public static IEnumerable<DateOnly> GetSundaysBetween(DateOnly start, DateOnly? end)
        {
            if (end == null)
            {
                end = DateOnly.FromDateTime(DateTime.Today);
            }

            if (end < start)
            {
                throw new ArgumentException("end 不能早于 start");
            }

            // 先找到 >= start 的第一个星期日
            int daysToSunday =
                ((int)DayOfWeek.Sunday - (int)start.DayOfWeek + 7) % 7;

            var firstSunday = start.AddDays(daysToSunday);

            for (var date = firstSunday; date <= end; date = date.AddDays(7))
            {
                yield return date;
            }
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

            int days = endDate.DayNumber - startDate.DayNumber + 1;
            var result = new DateOnly[days];

            for (int i = 0; i < days; i++)
            {
                result[i] = startDate.AddDays(i);
            }

            return result;
        }
        
        /// <summary>
        /// 检查传入N个日期DateOnly参数两两之间的日期差距，是否在指定误差之内。
        /// </summary>
        /// <param name="maxDays"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        /// <remarks>允许传入日期是null，null视为和任意日期没有差距。</remarks>
        /// <example>例如传入30天和3个DateOnly参数，如果任意两个日期之间的差距大于30，该方法返回false。</example>
        public static bool IsWithinDays(int maxDays, params DateOnly?[] dates)
        {
            for (int i = 0; i < dates.Length - 1; i++)
            {
                if (dates[i] is null)
                {
                    continue;
                }

                for (int j = i + 1; j < dates.Length; j++)
                {
                    if (dates[j] is null)
                    {
                        continue;
                    }

                    int days = Math.Abs(dates[i]!.Value.DayNumber - dates[j]!.Value.DayNumber);
                    if (days > maxDays)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
