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
        public BalanceHandler(Budget budget)
        {
            Budget = budget;
        }

        private Budget Budget { get; }
        public double DaysFromBudgetStart => (DateTime.Today - Budget.StartingDate).TotalDays;

        private IEnumerable<BudgetCategory> SpendingCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending);

        private IEnumerable<BudgetCategory> IncomeCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Income);

        private IEnumerable<BudgetCategory> SavingCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Saving);

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
            var budgeted = SpendingCategoriesBalance.Where(x => x.OverallBudgetBalance > 0).Select(x => x.OverallBudgetBalance).Sum();
            return CurrentFunds() - budgeted;
        }
    }
}