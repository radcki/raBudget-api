using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Extensions;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;

namespace WebApi.Helpers
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

        private BudgetCategory Category { get; }
        private Budget Budget { get; }
        private DateTime StartDate { get; }
        private DateTime EndDate { get; }

        private int DaysInPeriod => (EndDate - StartDate).Days;
        private int MonthsInPeriod => 12 * (EndDate.Year - StartDate.Year) + (EndDate.Month - StartDate.Month) + 1;

        public PeriodReportDto PeriodReport
        {
            get
            {
                var balance = new BudgetCategoryBalance(Category);
                var dto = new PeriodReportDto();
                dto.Category = Category.ToDto();
                dto.TransactionsSum = Category.Transactions
                                              .Where(x => x.TransactionDateTime >= StartDate && x.TransactionDateTime <= EndDate)
                                              .Sum(x => x.Amount);

                dto.AveragePerDay = dto.TransactionsSum / DaysInPeriod;
                dto.AveragePerMonth = dto.TransactionsSum / MonthsInPeriod;

                dto.BudgetAmount = balance.PeriodBudget(StartDate, EndDate);

                dto.AllocationsSum = Category.Allocations.Where(x => x.AllocationDateTime >= StartDate && x.AllocationDateTime <= EndDate)
                                             .Sum(x => x.Amount);
                return dto;
            }
        }

        public static PeriodReportDto GetPeriodReport(BudgetCategory category, DateTime startDate, DateTime endDate)
        {
            var instance = new BudgetCategoryReport(category, startDate, endDate);
            return instance.PeriodReport;
        }

        public PeriodMonthlyReport MonthByMonth
        {
            get
            {
                var data = new List<MonthReportDto>();

                var date = StartDate;
                while (date <= EndDate)
                {
                    var dto = new MonthReportDto();
                    dto.Year = date.Year;
                    dto.Month = date.Month;
                    var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    dto.TransactionsSum = Category.Transactions
                                                  .Where(x => x.TransactionDateTime.Year == date.Year
                                                              && x.TransactionDateTime.Month == date.Month)
                                                  .Sum(x => x.Amount);

                    dto.AllocationsSum = Category.Allocations
                                                 .Where(x => x.AllocationDateTime.Year == date.Year
                                                             && x.AllocationDateTime.Month == date.Month)
                                                 .Sum(x => x.Amount);

                    dto.AveragePerDay = dto.TransactionsSum / daysInMonth;
                    data.Add(dto);
                    date = date.AddMonths(1);
                }

                return new PeriodMonthlyReport()
                       {
                           Category = Category.ToDto(),
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

    public class MonthReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public double TransactionsSum { get; set; }
        public double AllocationsSum { get; set; }
        public double AveragePerDay { get; set; }
    }

    public class PeriodMonthlyReport
    {
        public BudgetCategoryDto Category { get; set; }
        public List<MonthReportDto> Data { get; set; }
    }

    public class PeriodReportDto
    {
        public BudgetCategoryDto Category { get; set; }

        public double TransactionsSum { get; set; }
        public double AllocationsSum { get; set; }
        public double BudgetAmount { get; set; }
        public double AveragePerDay { get; set; }
        public double AveragePerMonth { get; set; }
    }
}