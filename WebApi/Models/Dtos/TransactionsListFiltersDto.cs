using System;
using System.Collections.Generic;

namespace WebApi.Models.Dtos
{
    public class TransactionsListFiltersDto
    {
        #region Properties

        public int BudgetId { get; set; }
        public int? GroupCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<BudgetCategoryDto> Categories { get; set; }

        #endregion
    }
}