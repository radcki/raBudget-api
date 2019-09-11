using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebApi.Models.Enum;

namespace WebApi.Models.Entities
{
    public class TransactionSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int TransactionScheduleId { get; set; }

        [Required] public int BudgetCategoryId { get; set; }

        [Required] public string Description { get; set; }

        [Required] public double Amount { get; set; }

        [Required] public eFrequency Frequency { get; set; }

        [Required] public int PeriodStep { get; set; }

        [Required] public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

        /*
        * Klucz obce
        */

        public virtual BudgetCategory BudgetCategory { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
    }
}
