using System;

namespace raBudget.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public DateTime CreationDateTime { get; set; }

        public Guid CreatedByUserId { get; set; }

        public int BudgetCategoryId { get; set; }
        
        public int? TransactionScheduleId { get; set; }

        /*
         * Navigation properties
         */
        public virtual User CreatedByUser { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
        public virtual TransactionSchedule TransactionSchedule { get; set; }
    }
}