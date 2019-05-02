using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;

namespace WebApi.Helpers
{
    public class BalanceHandler
    {
        public BalanceHandler(Budget budget, eBudgetCategoryType limitCategoryType)
        {
            Budget = budget;
            Budget = budget;
            switch (limitCategoryType)
            {
                case eBudgetCategoryType.Spending:
                    SpendingCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending);
                    break;
                case eBudgetCategoryType.Income:
                    IncomeCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Income);
                    break;
                case eBudgetCategoryType.Saving:
                    SavingCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Saving);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(limitCategoryType), limitCategoryType, null);
            }
            
            DaysFromBudgetStart = (DateTime.Today - Budget.StartingDate).TotalDays;
        }
        public BalanceHandler(Budget budget)
        {
            Budget = budget;
            SpendingCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending);
            IncomeCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Income);
            SavingCategories = Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Saving);
            DaysFromBudgetStart = (DateTime.Today - Budget.StartingDate).TotalDays;
        }

        private Budget Budget { get; }
        public double DaysFromBudgetStart { get; }

        private IEnumerable<BudgetCategory> SpendingCategories { get; }

        private IEnumerable<BudgetCategory> IncomeCategories { get; }

        private IEnumerable<BudgetCategory> SavingCategories { get; }

        public double CurrentFunds() => IncomeCategories.TransactionsSum()
                                        - SpendingCategories.TransactionsSum()
                                        - SavingCategories.TransactionsSum();

        public static double CurrentFunds(Budget budget)
        {
            var handler = new BalanceHandler(budget);
            return handler.IncomeCategories.TransactionsSum()
                   - handler.SpendingCategories.TransactionsSum()
                   - handler.SavingCategories.TransactionsSum();
        }

        public List<BudgetCategoryBalanceDto> SpendingCategoriesBalance => SpendingCategories.Select(BudgetCategoryBalance.BalanceDto).ToList();
        public List<BudgetCategoryBalanceDto> IncomeCategoriesBalance => IncomeCategories.Select(BudgetCategoryBalance.BalanceDto).ToList();
        public List<BudgetCategoryBalanceDto> SavingCategoriesBalance => SavingCategories.Select(BudgetCategoryBalance.BalanceDto).ToList();

        public double UnassignedFunds()
        {
            var budgeted = SpendingCategories.Select(x=>BudgetCategoryBalance.BalanceDto(x,eBudgetCategoryBalanceIncluded.OverallBalance).OverallBudgetBalance).Where(x=>x > 0).Sum();
            return CurrentFunds() - budgeted;
        }
    }
}