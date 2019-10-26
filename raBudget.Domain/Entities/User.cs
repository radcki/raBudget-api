using System;
using System.Collections.Generic;

namespace raBudget.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public User()
        {
            CreationTime = DateTime.Now;
            OwnedBudgets = new HashSet<Budget>();
            BudgetShares = new HashSet<BudgetShare>();
        }
        public DateTime CreationTime { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }

        /*
         * Navigation properties
         */
        public virtual ICollection<Budget> OwnedBudgets { get; set; }
        public virtual ICollection<BudgetShare> BudgetShares { get; set; }
        public virtual ICollection<Transaction> RegisteredTransactions { get; set; }
        public virtual ICollection<TransactionSchedule> TransactionSchedules { get; set; }
        public virtual ICollection<Allocation> RegisteredAllocations{ get; set; }
    }
}