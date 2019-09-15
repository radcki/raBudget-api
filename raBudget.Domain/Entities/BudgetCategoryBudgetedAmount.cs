using System;

namespace raBudget.Domain.Entities
{
    public class  BudgetCategoryBudgetedAmount : BaseEntity<int>
    {

        public double MonthlyAmount { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int BudgetCategoryId{ get; set; }

        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}