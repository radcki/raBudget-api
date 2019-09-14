using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using raBudget.Domain.Enum;

namespace raBudget.Domain.Entities
{
    public class TransactionSchedule
    {
        public TransactionSchedule()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int TransactionScheduleId { get; set; }

        public int BudgetCategoryId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public eFrequency Frequency { get; set; }

        public int PeriodStep { get; set; }

        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }


        /*
         * Navigation properties
         */
        public virtual BudgetCategory BudgetCategory { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
