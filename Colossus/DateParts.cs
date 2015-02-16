using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus
{
    public class DateParts
    {
        public DateTime BaseDate { get; set; }

        public DateParts()
        {
            BaseDate = DateTime.Now;
        }

        public double? Hour { get; set; }
        public double? DayOfWeek { get; set; }
        public double? DayOfYear { get; set; }
        public double? Year { get; set; }


        public DateTime Date
        {
            get
            {
                var d = BaseDate;
                if (Year.HasValue)
                {
                    var y = (int) Math.Floor(Year.Value);
                    d = new DateTime(y, 1, 1).AddDays((Year.Value - y)*(DateTime.IsLeapYear(y) ? 366 : 365));
                }

                if (DayOfYear.HasValue)
                {
                    var days = (DayOfYear.Value/365)*(DateTime.IsLeapYear(d.Year) ? 366 : 365);
                    d = new DateTime(d.Year, 1, 1).AddDays(days);
                }

                if (DayOfWeek.HasValue)
                {
                    d = d.AddDays(-(int) d.DayOfWeek + DayOfWeek.Value);
                }

                if (Hour.HasValue)
                {
                    d = d.Date.AddHours(Hour.Value);
                }

                return d;
            }
        }

        public static implicit operator DateTime(DateParts parts)
        {
            return parts.Date;
        }
    }
}
