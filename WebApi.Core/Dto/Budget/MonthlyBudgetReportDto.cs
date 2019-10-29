using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Core.Dto.Budget
{
    public class MonthlyBudgetReportDto
    {
        public List<BudgetCategoryMonthlyReportDto> BudgetCategoryReports { get; set; }

        public IEnumerable<MonthReportDto> TotalMonthlyReports { get; set; }
    }

    public class Month : IEquatable<Month>
    {
        public int Year { get; set; }
        public int MonthNumber { get; set; }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Month other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Year == other.Year && MonthNumber == other.MonthNumber;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Month) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Year * 397) ^ MonthNumber;
            }
        }

        #endregion
    }


    public class BudgetCategoryMonthlyReportDto
    {
        public int BudgetCategoryId { get; set; }

        public IEnumerable<MonthReportDto> MonthlyReports { get; set; }
    }

    public class MonthReportDto
    {
        public Month Month { get; set; }
        public ReportDataDto ReportData { get; set; }
    }
}
