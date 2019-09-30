using System;
using System.Linq;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;
using WebApi.Models.Dtos;

namespace WebApi.Helpers
{
    public class BudgetCategoryBalance
    {
        /*
        public BudgetCategoryBalance(BudgetCategory category)
        {
            Category = category;
            Budget = category.Budget;

            ThisMonthAllocationsSum = Category
                                     .Allocations
                                     .Where(x => x.AllocationDateTime.Year == DateTime.Today.Year
                                                 && x.AllocationDateTime.Month == DateTime.Today.Month)
                                     .Sum(x => x.Amount);
            ThisMonthTransactionsSum = Category
                                      .Transactions
                                      .Where(x => x.TransactionDateTime.Year == DateTime.Today.Year
                                                  && x.TransactionDateTime.Month == DateTime.Today.Month)
                                      .Sum(x => x.Amount);
            TotalTransactionsSum = Category.TransactionsSum ?? Category.Transactions.Where(x => x.TransactionDateTime >= Budget.StartingDate).Sum(x => x.Amount);
            TotalAllocationsSum = Category.AllocationsSum ?? Category.Allocations.Where(x => x.AllocationDateTime >= Budget.StartingDate).Sum(x => x.Amount);
        }

        private BudgetCategory Category { get; }
        private Budget Budget { get; }

        public double ThisMonthTransactionsSum { get; }

        public double ThisMonthAllocationsSum { get; }

        public double ThisMonthYetScheduledSum
        {
            get
            {
                var schedules = Category.TransactionSchedules.Where(x => x.StartDate.Month <= DateTime.Now.Month && x.StartDate.Year <= DateTime.Now.Year);
                var today = DateTime.Today;
                double sum = 0;
                foreach (var schedule in schedules)
                {
                    var test = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                    var occurrences = schedule.OccurrencesInPeriod(today, new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1));
                    sum += schedule.Amount * occurrences.Count;
                }

                return sum;
            }
        }

        public double ThisYearYetScheduledSum
        {
            get
            {
                var schedules = Category.TransactionSchedules.Where(x => x.EndDate == null || x.EndDate >= DateTime.Today);
                var today = DateTime.Today;
                double sum = 0;
                foreach (var schedule in schedules)
                {
                    var occurrences = schedule.OccurrencesInPeriod(today, new DateTime(today.Year, 12, 31));
                    sum += schedule.Amount * occurrences.Count;
                }

                return sum;
            }
        }

        public double TotalTransactionsSum { get; }

        public double TotalAllocationsSum { get; }


        /// <summary>
        ///     Ogólna zabudżetowana w bieżącym roku kwota w kategorii
        /// </summary>
        public double ThisYearBudget => Category.Allocations.Where(x => DateTime.Now.Year == x.AllocationDateTime.Year).Sum(x => x.Amount) //alokacje z tego roku
                                        + (DateTime.Now.Year == Budget.StartingDate.Year
                                               ? PeriodBudget(Budget.StartingDate, new DateTime(DateTime.Today.Year, 12, 1))
                                               : PeriodBudget(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 1)));

        /// <summary>
        ///     Ogólna zabudżetowana kwota od początku budżetu
        /// </summary>
        public double BudgetSoFar => TotalAllocationsSum + PeriodBudget(Budget.StartingDate, DateTime.Today);

        public double ThisMonthBudget => ThisMonthAllocationsSum + PeriodBudget(DateTime.Today.FirstDayOfMonth(), DateTime.Today.LastDayOfMonth());


        public double PeriodBudget(DateTime from, DateTime to)
        {
            var configPeriods = Category.BudgetCategoryAmountConfigs.Where(x => (x.ValidTo ?? DateTime.MaxValue) >= from && x.ValidFrom <= to);
            double budget = 0;
            foreach (var config in configPeriods)
            {
                var monthsCount = OverlapingMonths(from, to, config.ValidFrom, config.ValidTo ?? DateTime.MaxValue);
                budget += monthsCount * config.MonthlyAmount;
            }

            return budget;
        }

        /// <summary>
        ///     Bilans kategorii
        /// </summary>
        public double OverallBudgetBalance => BudgetSoFar - TotalTransactionsSum;

        public double ThisMonthBudgetBalance => ThisMonthBudget - ThisMonthTransactionsSum;

        public double LeftToEndOfYear => ThisYearBudget - Category.Transactions.Where(x => x.TransactionDateTime.Year == DateTime.Today.Year).Sum(x => x.Amount);

        public static BudgetCategoryBalanceDto BalanceDto(BudgetCategory category)
        {
            return BalanceDto(category, eBudgetCategoryBalanceIncluded.All);
        }

        public static BudgetCategoryBalanceDto BalanceDto(BudgetCategory category, eBudgetCategoryBalanceIncluded included)
        {
            var balance = new BudgetCategoryBalance(category);
            var dto = new BudgetCategoryBalanceDto() {BudgetCategory = balance.Category.ToDto()};
            if (included.HasFlag(eBudgetCategoryBalanceIncluded.OverallBalance))
            {
                dto.OverallBudgetBalance = balance.OverallBudgetBalance;
            }

            if (included.HasFlag(eBudgetCategoryBalanceIncluded.ThisMonth))
            {
                dto.ThisMonthBudgetBalance = balance.ThisMonthBudgetBalance;
                dto.ThisMonthBudget = balance.ThisMonthBudget;
                dto.ThisMonthTransactionsSum = balance.ThisMonthTransactionsSum;
                dto.ThisMonthYetScheduledSum = balance.ThisMonthYetScheduledSum();
            }

            if (included.HasFlag(eBudgetCategoryBalanceIncluded.ThisYear))
            {
                dto.LeftToEndOfYear = balance.LeftToEndOfYear;
                dto.ThisYearBudget = balance.ThisYearBudget;
                dto.ThisYearYetScheduledSum = balance.ThisYearYetScheduledSum();
            }

            if (included.HasFlag(eBudgetCategoryBalanceIncluded.Totals))
            {
                dto.TotalTransactionsSum = balance.TotalTransactionsSum;
                dto.TotalAllocationsSum = balance.TotalAllocationsSum;
                dto.OverallBudgetBalance = balance.OverallBudgetBalance;
                dto.BudgetSoFar = balance.BudgetSoFar;
            }

            return dto;
        }

        private static int OverlapingMonths(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        {
            if (!(s1 <= e2 && e1 >= s2))
            {
                return 0;
            }

            DateTime start = s1 > s2 ? s1 : s2;
            DateTime end = e1 > e2 ? e2 : e1;

            return (12 * (end.Year - start.Year) + end.Month - start.Month) + 1;
        }
        */
    }
}