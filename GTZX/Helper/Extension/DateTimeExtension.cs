using System;

namespace Helper.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="formatString">
        /// 日期格式，自定义日期格式说明：
        /// y：不包含纪元的年份。如果不包含纪元的年份小于10，则显示不具有前导0的年份；
        /// yy：不包含纪元的年份。如果不包含纪元的年份小于10，则显示具有前导0的年份；
        /// yyyy：包含纪元的4位数年份；
        /// M：月份数字。一位数的月份没有前导0；
        /// MM：月份数字。一位数的月份有前导0；
        /// MMM：月份的缩写名称；
        /// d：月中的某一天。一位数的日期没有前导0；
        /// dd：月份中的某一天。一位数的日期有前导0；
        /// h：12小时制的小时。一位数的小时没有前导0；
        /// hh：12小时制的小时。一位数的小时有前导0；
        /// H：24小时制的小时。一位数的小时没有前导0；
        /// HH：24小时制的小时。一位数的小时有前导0；
        /// m：分钟数。一位数的分钟数没有前导0；
        /// mm：分钟数。一位数的分钟数有前导0；
        /// s：秒数。一位数的秒数没有前导0；
        /// ss：秒数。一位数的秒数有前导0；
        /// f：秒的小数精度为1位，其余被截断；同理小数精度为几位，就是几个f；
        /// tt：AM/PM
        /// z：时区，如+8
        /// zz：时区，如+08
        /// </param>
        /// <returns>格式化的日期字符串</returns>
        public static string Format(this DateTime dateTime, string formatString = "yyyy/MM/dd HH:mm:ss.fff")
        {
            return dateTime.ToString(formatString);
        }

        public static string Format(this DateTime? dateTime, string formatString = "yyyy/MM/dd HH:mm:ss.fff")
        {
            if (!dateTime.HasValue) return string.Empty;
            return dateTime.Value.Format(formatString);
        }
    }
}
