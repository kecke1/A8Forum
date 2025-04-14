using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions;
public static class DateTimeExtensions
{

    /// <summary>
    /// Copied (with small modifications) from here: https://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates
    /// Calculates number of business days, taking into account:
    ///  - weekends (Saturdays and Sundays)
    ///  - bank holidays in the middle of the week
    /// </summary>
    /// <param name="firstDay">First day in the time interval</param>
    /// <param name="lastDay">Last day in the time interval</param>
    /// <param name="excludesDays">List of excluded week days</param>
    /// <returns>Number of business days during the 'span'</returns>
    public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] excludesDays)
    {
        firstDay = firstDay.Date;
        lastDay = lastDay.Date;
        if (firstDay > lastDay)
            throw new ArgumentException("Incorrect last day " + lastDay);

        var span = lastDay - firstDay;
        var businessDays = span.Days + 1;
        var fullWeekCount = businessDays / 7;
        // find out if there are weekends during the time exceeding the full weeks
        if (businessDays > fullWeekCount * 7)
        {
            // we are here to find out if there is a 1-day or 2-days weekend
            // in the time interval remaining after subtracting the complete weeks
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            var lastDayOfWeek = (int)lastDay.DayOfWeek;
            if (lastDayOfWeek < firstDayOfWeek)
                lastDayOfWeek += 7;
            if (firstDayOfWeek <= 6)
            {
                if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                    businessDays -= 2;
                else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                    businessDays -= 1;
            }
            else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                businessDays -= 1;
        }

        // subtract the weekends during the full weeks in the interval
        businessDays -= fullWeekCount + fullWeekCount;

        // subtract the number of bank holidays during the time interval
        foreach (var e in excludesDays)
        {
            var ed = e.Date;
            if (firstDay <= ed && ed <= lastDay)
                --businessDays;
        }

        return businessDays;
    }
}
