using System;

namespace raBudget.Domain.Entities
{
    public class Allocation : BaseEntity<int>
    {
        public string Description { get; set; }

        public double Amount { get; set; }

        public DateTime AllocationDateTime { get; set; }

        public DateTime CreationDateTime { get; set; }

        public Guid CreatedByUserId { get; set; }

        public int TargetBudgetCategoryId { get; set; }
        public int? SourceBudgetCategoryId { get; set; }

        /*
         * Navigation properties
         */
        public virtual User CreatedByUser { get; set; }
        public virtual BudgetCategory TargetBudgetCategory { get; set; }
        public virtual BudgetCategory SourceBudgetCategory { get; set; }
    }
}