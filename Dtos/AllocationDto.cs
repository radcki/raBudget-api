using System;

namespace WebApi.Dtos
{
    public class AllocationDto
    {
        public int AllocationId { get; set; }
        public BudgetCategoryDto SourceCategory { get; set; }
        public BudgetCategoryDto DestinationCategory { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDataDto Budget { get; set; }
        public DateTime Date { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}