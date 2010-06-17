using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Holidays
{
    public interface IGenerateHolidays
    {
        IEnumerable<Holiday> GenerateHolidays(DateTime inclusiveStart, DateTime inclusiveEnd);
    }
}
