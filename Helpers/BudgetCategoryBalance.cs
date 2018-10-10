using System;
using System.Linq;
using WebApi.Dtos;
using WebApi.Entities;

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
                                                 .Where(x => x.TransactionDateTime.Month == DateTime.Today.Month)
                                                 .Sum(x => x.Amount);

        public double ThisMonthAllocationsSum => Category
                                                .Allocations
                                                .Where(x => x.AllocationDateTime.Month == DateTime.Today.Month)
                                                .Sum(x => x.Amount);

        public double TotalTransactionsSum => Category.Transactions.Where(x => x.TransactionDateTime >= Budget.StartingDate).Sum(x => x.Amount);

        public double TotalAllocationsSum => Category.Allocations.Where(x => x.AllocationDateTime >= Budget.StartingDate).Sum(x => x.Amount);

        /// <summary>
        ///     Zabudżetowana kwota w pierwszym roku budżetu
        /// </summary>
        public double FirstYearBudget
        {
            get
            {
                /*
                 * np start w sierpniu - 12 - 8 = 4 pełne miesiące (wrzesień-grudzień)
                 */
                var fullMonths = 12 - Budget.StartingDate.Month;
                /*
                 * Pozostałość z potencjalnie niepełnego miesiąca
                 * np 1 sierpnia - 31-1+1 = 31, 5 sierpnia - 31-5+1 = 27
                 */
                var leftoverDays = DateTime.DaysInMonth(Budget.StartingDate.Year, Budget.StartingDate.Month)
                                   - Budget.StartingDate.Day + 1;
                return fullMonths * Category.MonthlyAmount
                       + leftoverDays * (Category.MonthlyAmount / leftoverDays);
            }
        }

        /// <summary>
        ///     Ogólna zabudżetowana w bieżącym roku kwota w kategorii
        /// </summary>
        public double ThisYearBudget => Category.Allocations.Where(x=>DateTime.Now.Year == x.AllocationDateTime.Year).Sum(x=>x.Amount) //alokacje z tego roku
                                        + (DateTime.Now.Year == Budget.StartingDate.Year
                                            ? FirstYearBudget
                                            : Category.MonthlyAmount * 12);

        /// <summary>
        ///     Ogólna zabudżetowana kwota od początku budżetu
        /// </summary>
        public double BudgetSoFar
        {
            get
            {
                var firstMonthDays = DateTime.DaysInMonth(Budget.StartingDate.Year, Budget.StartingDate.Month);
                return (Category.MonthlyAmount / firstMonthDays) * (firstMonthDays - Budget.StartingDate.Day + 1) // obliczanie dla potencjalnie niepełnego miesiąca
                       + TotalAllocationsSum // dodanie alokacji
                       + Category.MonthlyAmount * ((DateTime.Today.Year - Budget.StartingDate.Year) * 12 + DateTime.Today.Month - Budget.StartingDate.Month); // dodanie pełnych miesięcy
            }
        }

        public double ThisMonthBudget
        {
            get
            {
                if (DateTime.Now.Year != Budget.StartingDate.Year || DateTime.Now.Month != Budget.StartingDate.Month)
                {
                    return Category.MonthlyAmount;
                }

                var leftoverDays = DateTime.DaysInMonth(Budget.StartingDate.Year, Budget.StartingDate.Month)
                                   - Budget.StartingDate.Day + 1;
                return ThisMonthAllocationsSum 
                       + leftoverDays * (Category.MonthlyAmount / DateTime.DaysInMonth(Budget.StartingDate.Year, Budget.StartingDate.Month));
            }
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
                       BudgetCategory = new BudgetCategoryDto()
                                        {
                                            Amount = balance.Category.MonthlyAmount,
                                            CategoryId = balance.Category.BudgetCategoryId,
                                            Icon = balance.Category.Icon,
                                            Name = balance.Category.Name,
                                            Type = balance.Category.Type
                                        },
                       LeftToEndOfYear = balance.LeftToEndOfYear,
                       ThisYearBudget = balance.ThisYearBudget,
                       ThisMonthTransactionsSum = balance.ThisMonthTransactionsSum,
                   };
        }
    }
}