namespace WebApi.Models.Dtos
{
    public class BudgetCategoryBalanceDto
    {
        #region Properties

        public BudgetCategoryDto BudgetCategory { get; set; }

        public double OverallBudgetBalance { get; set; }
        public double ThisMonthBudgetBalance { get; set; }

        public double ThisMonthTransactionsSum { get; set; }
        public double ThisMonthYetScheduledSum { get; set; }

        public double LeftToEndOfYear { get; set; }

        public double ThisYearBudget { get; set; }
        public double ThisYearYetScheduledSum { get; set; }
        public double ThisMonthBudget { get; set; }
        public double BudgetSoFar { get; set; }

        public double TotalTransactionsSum { get; set; }
        public double TotalAllocationsSum { get; set; }

        #endregion
    }
}