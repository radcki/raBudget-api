namespace WebApi.Helpers
{
    public class ReportHandler
    {
        /*
        public ReportHandler(Budget budget, DateTime startDate, DateTime endDate)
        {
            Budget = budget;
            StartDate = startDate;
            EndDate = endDate;
        }

        private Budget Budget { get; }
        private DateTime StartDate { get; }
        private DateTime EndDate { get; }


        private IEnumerable<BudgetCategory> SpendingCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending);

        private IEnumerable<BudgetCategory> IncomeCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Income);

        private IEnumerable<BudgetCategory> SavingCategories =>
            Budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Saving);


        public List<PeriodReportDto> SpendingCategoriesSummary => SpendingCategories.Select( x => BudgetCategoryReport.GetPeriodReport(x,StartDate,EndDate)).ToList();
        public List<PeriodReportDto> IncomeCategoriesSummary => IncomeCategories.Select( x => BudgetCategoryReport.GetPeriodReport(x,StartDate,EndDate)).ToList();
        public List<PeriodReportDto> SavingCategoriesSummary => SavingCategories.Select( x => BudgetCategoryReport.GetPeriodReport(x,StartDate,EndDate)).ToList();

        public List<PeriodMonthlyReport> SpendingCategoriesByMonth => SpendingCategories.Select(x => BudgetCategoryReport.GetMonthByMonth(x, StartDate, EndDate)).ToList();
        public List<PeriodMonthlyReport> IncomeCategoriesByMonth => IncomeCategories.Select(x => BudgetCategoryReport.GetMonthByMonth(x, StartDate, EndDate)).ToList();
        public List<PeriodMonthlyReport> SavingCategoriesByMonth => SavingCategories.Select(x => BudgetCategoryReport.GetMonthByMonth(x, StartDate, EndDate)).ToList();
    }
    */
    }
}