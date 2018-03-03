using System;

namespace BudgetCalculator
{
    public class Budget
    {
        public string YearMonth { get; set; }

        public int Amount { get; set; }

        public int TotalDays()
        {
            var dateTime = DateTime.ParseExact(YearMonth + "01", "yyyyMMdd", null);
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        public int DailyAmount()
        {
            return Amount / TotalDays();
        }
    }
}