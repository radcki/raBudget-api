using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Core.Dto.Budget
{
    public class PeriodBudgetReportDto
    {
        public List<BudgetCategoryPeriodReportDto> BudgetCategoryReports { get; set; }

        public ReportDataDto PeriodBudgetReport { get; set; }
    }

    
    public class BudgetCategoryPeriodReportDto
    {
        public int BudgetCategoryId { get; set; }

        public ReportDataDto PeriodCategoryReport { get; set; }
    }
}
