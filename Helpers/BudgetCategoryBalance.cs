using System;
using System.Linq;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;

namespace WebApi.Helpers
{
    public class BudgetCategoryBalance
    {
        public BudgetCategoryBalance(BudgetCategory category)
        {
            Category = category;
            Budget = category.Budget;
        }

        private BudgetCategory Category { get; }
        private Budget Budget { get; }

        public double ThisMonthTransactionsSum => Category
                                                 .Transactions
                                                 .Where(x => x.TransactionDateTime.Year == DateTime.Today.Year 
                                                             && x.TransactionDateTime.Month == DateTime.Today.Month)
                                                 .Sum(x => x.Amount);

        public double ThisMonthAllocationsSum => Category
                                                .Allocations
                                                .Where(x => x.AllocationDateTime.Year == DateTime.Today.Year
                                                            && x.AllocationDateTime.Month == DateTime.Today.Month)
                                                .Sum(x => x.Amount);

        public double TotalTransactionsSum => Category.Transactions.Where(x => x.TransactionDateTime >= Budget.StartingDate).Sum(x => x.Amount);

        public double TotalAllocationsSum => Category.Allocations.Where(x => x.AllocationDateTime >= Budget.StartingDate).Sum(x => x.Amount);

       
        /// <summary>
        ///     Ogólna zabudżetowana w bieżącym roku kwota w kategorii
        /// </summary>
        public double ThisYearBudget => Category.Allocations.Where(x=>DateTime.Now.Year == x.AllocationDateTime.Year).Sum(x=>x.Amount) //alokacje z tego roku
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
            var configPeriods = Category.BudgetCategoryAmountConfigs.Where(x => (x.ValidTo??DateTime.MaxValue) >= from && x.ValidFrom <= to);
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

        public double ThisMonthBudgetBalance => ThisMonthBudget - ThisMonthTransactionsSum ;

        public double LeftToEndOfYear => ThisYearBudget - Category.Transactions.Where(x => x.TransactionDateTime.Year == DateTime.Today.Year).Sum(x => x.Amount);

        public static BudgetCategoryBalanceDto BalanceDto(BudgetCategory category)
        {
            var balance = new BudgetCategoryBalance(category);
            return new BudgetCategoryBalanceDto()
                   {
                       TotalTransactionsSum = balance.TotalTransactionsSum,
                       TotalAllocationsSum = balance.TotalAllocationsSum,
                       ThisMonthBudgetBalance = balance.ThisMonthBudgetBalance,
                       OverallBudgetBalance = balance.OverallBudgetBalance,
                       ThisMonthBudget = balance.ThisMonthBudget,
                       BudgetSoFar = balance.BudgetSoFar,
                       BudgetCategory = balance.Category.ToDto(),
                       LeftToEndOfYear = balance.LeftToEndOfYear,
                       ThisYearBudget = balance.ThisYearBudget,
                       ThisMonthTransactionsSum = balance.ThisMonthTransactionsSum,
                   };
        }

        private static int OverlapingMonths(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        {
            if (!(s1 <= e2 && e1 >= s2))
            {
                return 0;
            }
            DateTime start = s1 > s2 ? s1 : s2;
            DateTime end = e1 > e2 ? e2 : e1;

            return (12*(end.Year-start.Year) + end.Month - start.Month) + 1;
        }

    }
}