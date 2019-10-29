using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Core.Dto.Budget
{
    public class ReportDataDto
    {
        public double BudgetedSum { get; set; }
        public double TransactionsSum { get; set; }
        public double AllocationsSum { get; set; }
        public double AveragePerDay { get; set; }
    }
}
