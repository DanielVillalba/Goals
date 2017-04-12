using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class DateHelper
    {
        public static DateTime GetMonthMaxDate(DateTime baseDate)
        {
            return new DateTime(baseDate.Year, baseDate.Month, 1).AddMonths(1).AddMilliseconds(-1);
        }

        public static bool IsInRange(this DateTime thisDate, DateTime minDate, DateTime maxDate)
        {
            return thisDate >= minDate && thisDate <= maxDate;
        }
    }
}
