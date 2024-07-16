namespace HappreeTool.CommonUtils
{
    internal class DateUtils
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

    }
}
