using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Domain.Entities
{
    public class BudgetCategoryBalance
    {
        public BudgetCategoryBalance(BudgetCategory category)
        {
            BudgetCategoryId = category.Id;
            OverallBudgetBalance = category.OverallBudgetBalance;
            ThisMonthBudgetBalance = category.ThisMonthBudgetBalance;
            ThisMonthTransactionsSum = category.ThisMonthTransactionsSum;
            ThisMonthYetScheduledSum = category.ThisMonthYetScheduledSum;
            LeftToEndOfYear = category.LeftToEndOfYear;
            ThisYearBudget = category.ThisYearBudget;
            ThisYearYetScheduledSum = category.ThisYearYetScheduledSum;
            ThisMonthBudget = category.ThisMonthBudget;
            BudgetSoFar = category.BudgetSoFar;

            TotalTransactionsSum = category.TotalTransactionsSum;
            TotalAllocationsSum = category.TotalAllocationsSum;
        }

        public int BudgetCategoryId { get; }

        public double OverallBudgetBalance { get; }
        public double ThisMonthBudgetBalance { get; }

        public double ThisMonthTransactionsSum { get;}
        public double ThisMonthYetScheduledSum { get; }

        public double LeftToEndOfYear { get; }

        public double ThisYearBudget { get; }
        public double ThisYearYetScheduledSum { get; }
        public double ThisMonthBudget { get; }
        public double BudgetSoFar { get; }

        public double TotalTransactionsSum { get; }
        public double TotalAllocationsSum { get; }
    }
}