using System;
using System.Globalization;

namespace Equinor.Procosys.Preservation.Domain
{
    public class PreservationDate
    {
        private DateTime _time;
        private int _year;
        private int _week;

        public PreservationDate(DateTime time)
        {
            _time = time;
            _year = ISOWeek.GetYear(time);
            _week = ISOWeek.GetWeekOfYear(time);
        }

        public override string ToString() => string.Concat(_year.ToString(), "W", _week.ToString("00"));
    }
}
