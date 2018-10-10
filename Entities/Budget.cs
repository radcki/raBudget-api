using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Budget
    {
        [Key] public int BudgetId { get; set; }

        public string Name { get; set; }
        public string Currency { get; set; }
        public DateTime StartingDate { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        /*
         * Klucze obce
         */
        public virtual List<BudgetCategory> BudgetCategories { get; set; }
    }
}