using System;
using System.Collections.Generic;
using raBudget.Domain.Enum;

namespace raBudget.Domain.Entities
{
    public class Budget : BaseEntity<int>
    {
        public Budget()
        {
            BudgetCategories = new HashSet<BudgetCategory>();
            BudgetShares = new HashSet<BudgetShare>();
        }

        public Budget(int budgetId)
        {
            BudgetCategories = new HashSet<BudgetCategory>();
            BudgetShares = new HashSet<BudgetShare>();
            Id = budgetId;
        }

        public string Name { get; set; }
        public eCurrency CurrencyCode { get; set; }
        public virtual Currency Currency => Currency.Get(CurrencyCode);
        public DateTime StartingDate { get; set; }

        public Guid OwnedByUserId { get; set; }
        public virtual User OwnedByUser { get; set; }

        /*
         * Navigation properties
         */
        public ICollection<BudgetCategory> BudgetCategories { get; set; }
        public ICollection<BudgetShare> BudgetShares { get; set; }
    }
}