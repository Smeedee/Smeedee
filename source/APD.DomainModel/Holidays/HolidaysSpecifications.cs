using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using APD.DomainModel.Framework;


namespace APD.DomainModel.Holidays
{
    public class HolidaySpecification : Specification<Holiday>
    {
        private DateTime startDate = DateTime.MinValue.Date;
        private DateTime endDate = DateTime.MaxValue.Date;
        public List<DayOfWeek> NonWorkingDaysOfWeek { get; set; }

        public DateTime StartDate
        {
            get { return startDate; }
            set { 
                    if( value > EndDate) 
                        throw new ArgumentException("Start date cannot be after end date");
                    else
                        startDate = value; 
                }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (value < StartDate)
                    throw new ArgumentException("End date cannot be before start date");
                else
                    endDate = value;
            }
        }

        public HolidaySpecification()
        {
            NonWorkingDaysOfWeek = new List<DayOfWeek>();
        }

        public override Expression<Func<Holiday, bool>> IsSatisfiedByExpression()
        {
            return h => h.Date >= StartDate && h.Date <= EndDate;
        }
    }
}
