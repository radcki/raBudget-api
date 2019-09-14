using System.Collections.Generic;
using raBudget.Domain.Enum;

namespace raBudget.Domain.Entities
{
    public class BudgetCategory
    {
        public BudgetCategory()
        {
            Transactions = new HashSet<Transaction>();
            Allocations = new HashSet<Allocation>();
            BudgetCategoryBudgetedAmounts = new HashSet<BudgetCategoryBudgetedAmount>();
            TransactionSchedules = new HashSet<TransactionSchedule>();
        }
        public int BudgetCategoryId { get; set; }

        public eBudgetCategoryType Type { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }
            
        public int BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        /*
         * Navigation properties
         */
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Allocation> Allocations { get; set; }
        public ICollection<BudgetCategoryBudgetedAmount> BudgetCategoryBudgetedAmounts { get; set; }
        public ICollection<TransactionSchedule> TransactionSchedules { get; set; }
    }
}