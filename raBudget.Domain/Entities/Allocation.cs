using System;

namespace raBudget.Domain.Entities
{
    public class Allocation
    {
        public int AllocationId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public DateTime AllocationDateTime { get; set; }

        public DateTime CreationDateTime { get; set; }

        public Guid CreatedByUserId { get; set; }

        public int BudgetCategoryId { get; set; }

        /*
         * Navigation properties
         */
        public virtual User CreatedByUser { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}