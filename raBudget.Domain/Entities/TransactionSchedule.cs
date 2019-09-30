using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

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

        #region Business logic


        public List<DateTime> OccurrencesInPeriod( DateTime from, DateTime to)
        {
            var start = new[] { StartDate, from }.Max();
            var end = EndDate == null ? to : new[] { EndDate.Value, to }.Min();

            var allOccurrences = new List<DateTime>();
            var current = new DateTime(StartDate.Ticks);
            bool exitLoop = false;

            while (current <= end)
            {
                allOccurrences.Add(current);
                switch (Frequency)
                {
                    case eFrequency.Monthly:
                        current = current.AddMonths(PeriodStep);
                        break;
                    case eFrequency.Weekly:
                        current = current.AddDays(7 * PeriodStep);
                        break;
                    case eFrequency.Daily:
                        current = current.AddDays(PeriodStep);
                        break;
                    default:
                    case eFrequency.Once: //już dodany przed switch
                        exitLoop = true;
                        break;
                }

                if (exitLoop)
                {
                    break;
                }
            }

            return allOccurrences.Where(x => x >= start).Distinct().ToList();
        }

        #endregion
    }
}
