using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.Enum;

namespace WebApi.Entities
{
    public class BudgetCategory
    {
        [Key] public int BudgetCategoryId { get; set; }

        [Required] public eBudgetCategoryType Type { get; set; }

        [Required] public string Icon { get; set; }

        [Required] public string Name { get; set; }

        [Required] public double MonthlyAmount { get; set; }


        [Required] public int BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        /*
         * Klucz obce
         */
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<Allocation> Allocations { get; set; }
    }
}