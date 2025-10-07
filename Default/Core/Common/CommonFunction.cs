using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Core.Common
{
    public static class CommonFunction
    {
        public static List<DayOfWeek> GetWeekdaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            List<DayOfWeek> weekdays = new List<DayOfWeek>();

            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                weekdays.Add(date.DayOfWeek);
            }

            return weekdays;
        }
    }
}
