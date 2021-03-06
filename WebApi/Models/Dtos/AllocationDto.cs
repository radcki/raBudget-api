using System;

namespace WebApi.Models.Dtos
{
    public class AllocationDto
    {
        #region Properties

        public int AllocationId { get; set; }
        public BudgetCategoryDto SourceCategory { get; set; }
        public BudgetCategoryDto DestinationCategory { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDataDto Budget { get; set; }
        public DateTime Date { get; set; }
        public DateTime RegisteredDate { get; set; }

        #endregion
    }
}