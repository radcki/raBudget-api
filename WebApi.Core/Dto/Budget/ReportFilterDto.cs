using System;

namespace raBudget.Core.Dto.Budget
{
    public class ReportFilterDto
    {
        public ReportFilterDto()
        {
            DateEndFilter = new DateTime();
        }
        public DateTime DateStartFilter { get; set; }
        public DateTime DateEndFilter { get; set; }

    }
}
