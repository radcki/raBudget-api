using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using raBudget.Domain.Entities;

namespace raBudget.Domain.Entities
{
    public class BudgetCategoryReport
    {
        public BudgetCategoryReport(BudgetCategory category, DateTime startDate, DateTime endDate)
        {
            Category = category;
            Budget = category.Budget;
            StartDate = startDate;
            EndDate = endDate;
        }

        public readonly BudgetCategory Category;
        private Budget Budget { get; }
        private DateTime StartDate { get; }
        private DateTime EndDate { get; }

        private int DaysInPeriod => (EndDate - StartDate).Days;
        private int MonthsInPeriod => 12 * (EndDate.Year - StartDate.Year) + (EndDate.Month - StartDate.Month) + 1;

        public PeriodReport PeriodReport
        {
            get
            {
                var report = new PeriodReport();
                report.TransactionsSum = Category.Transactions
                                              .Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate)
                                              .Sum(x => x.Amount);

                report.AveragePerDay = report.TransactionsSum / DaysInPeriod;
                report.AveragePerMonth = report.TransactionsSum / MonthsInPeriod;

                report.BudgetAmount = Category.PeriodBudget(StartDate, EndDate);

                report.AllocationsSum = Category.TargetAllocations
                                             .Where(x => x.AllocationDateTime >= StartDate && x.AllocationDateTime <= EndDate)
                                             .Sum(x => x.Amount)
                                     - Category.SourceAllocations
                                               .Where(x => x.AllocationDateTime >= StartDate && x.AllocationDateTime <= EndDate)
                                               .Sum(x => x.Amount);

                return report;
            }
        }

        public static PeriodReport GetPeriodReport(BudgetCategory category, DateTime startDate, DateTime endDate)
        {
            var instance = new BudgetCategoryReport(category, startDate, endDate);
            return instance.PeriodReport;
        }

        public PeriodMonthlyReport MonthByMonth
        {
            get
            {
                var data = new List<MonthReport>();

                var date = StartDate;
                while (date <= EndDate)
                {
                    var report = new MonthReport();
                    report.Year = date.Year;
                    report.Month = date.Month;
                    var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    report.TransactionsSum = Category.Transactions
                                                  .Where(x => x.TransactionDateTime.Year == date.Year
                                                              && x.TransactionDateTime.Month == date.Month)
                                                  .Sum(x => x.Amount);

                    report.AllocationsSum = Category.TargetAllocations
                                                 .Where(x => x.AllocationDateTime.Year == date.Year
                                                             && x.AllocationDateTime.Month == date.Month)
                                                 .Sum(x => x.Amount)
                                         - Category.SourceAllocations
                                                   .Where(x => x.AllocationDateTime.Year == date.Year
                                                               && x.AllocationDateTime.Month == date.Month)
                                                   .Sum(x => x.Amount);

                    report.AveragePerDay = report.TransactionsSum / daysInMonth;
                    data.Add(report);
                    date = date.AddMonths(1);
                }

                return new PeriodMonthlyReport()
                       {
                           Category = Category,
                           Data = data
                       };
            }
        }

        public static PeriodMonthlyReport GetMonthByMonth(BudgetCategory category, DateTime startDate, DateTime endDate)
        {
            var instance = new BudgetCategoryReport(category, startDate, endDate);
            return instance.MonthByMonth;
        }
    }

    public class MonthReport
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public double TransactionsSum { get; set; }
        public double AllocationsSum { get; set; }
        public double AveragePerDay { get; set; }
    }

    public class PeriodMonthlyReport
    {
        public BudgetCategory Category { get; set; }
        public List<MonthReport> Data { get; set; }
    }

    public class PeriodReport
    {
        public BudgetCategory Category { get; set; }

        public double TransactionsSum { get; set; }
        public double AllocationsSum { get; set; }
        public double BudgetAmount { get; set; }
        public double AveragePerDay { get; set; }
        public double AveragePerMonth { get; set; }
    }
}