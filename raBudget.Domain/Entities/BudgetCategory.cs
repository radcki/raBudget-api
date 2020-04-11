using System;
using System.Collections.Generic;
using System.Linq;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Domain.Entities
{
    public class BudgetCategory : BaseEntity<int>
    {
        #region Constructors

        public BudgetCategory()
        {
            Transactions = new HashSet<Transaction>();
            TargetAllocations = new HashSet<Allocation>();
            SourceAllocations = new HashSet<Allocation>();
            BudgetCategoryBudgetedAmounts = new HashSet<BudgetCategoryBudgetedAmount>();
            TransactionSchedules = new HashSet<TransactionSchedule>();
        }

        #endregion

        #region Properties

        public eBudgetCategoryType Type { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public int BudgetId { get; set; }
        public int Order { get; set; }

        public virtual Budget Budget { get; set; }

        /*
         * Navigation properties
         */
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Allocation> TargetAllocations { get; set; }
        public ICollection<Allocation> SourceAllocations { get; set; }
        public ICollection<BudgetCategoryBudgetedAmount> BudgetCategoryBudgetedAmounts { get; set; }
        public ICollection<TransactionSchedule> TransactionSchedules { get; set; }

        #endregion

        #region Business logic

        private double _totalTransactionsSum;

        public double TotalTransactionsSum
        {
            get
            {
                if (Math.Abs(_totalTransactionsSum - (double) default) < 0.01)
                {
                    _totalTransactionsSum = Transactions.Where(x => x.TransactionDateTime >= Budget.StartingDate)
                                                        .Sum(x => x.Amount);
                }

                return _totalTransactionsSum;
            }
        }

        private double _totalAllocationsSum;

        public double TotalAllocationsSum
        {
            get
            {
                // Prevent reevaluation
                if (Math.Abs(_totalAllocationsSum - (double) default) < 0.01)
                {
                    _totalAllocationsSum = TargetAllocations.Where(x => x.AllocationDateTime >= Budget.StartingDate)
                                                            .Sum(x => x.Amount)
                                           - SourceAllocations.Where(x => x.AllocationDateTime >= Budget.StartingDate)
                                                              .Sum(x => x.Amount);
                }

                return _totalAllocationsSum;
            }
        }


        public double Balance => TotalTransactionsSum + TotalAllocationsSum;


        public double ThisMonthTransactionsSum => Transactions.Where(x => x.TransactionDateTime.Year == DateTime.Today.Year
                                                                          && x.TransactionDateTime.Month == DateTime.Today.Month)
                                                              .Sum(x => x.Amount);


        public double ThisMonthAllocationsSum => TargetAllocations.Where(x => x.AllocationDateTime.Year == DateTime.Today.Year
                                                                              && x.AllocationDateTime.Month == DateTime.Today.Month)
                                                                  .Sum(x => x.Amount)
                                                 - SourceAllocations.Where(x => x.AllocationDateTime.Year == DateTime.Today.Year
                                                                                && x.AllocationDateTime.Month == DateTime.Today.Month)
                                                                    .Sum(x => x.Amount);


        public double ThisYearBudget => TargetAllocations.Where(x => DateTime.Now.Year == x.AllocationDateTime.Year)
                                                         .Sum(x => x.Amount) // this year allocations
                                        - SourceAllocations.Where(x => DateTime.Now.Year == x.AllocationDateTime.Year)
                                                           .Sum(x => x.Amount) // this year allocations
                                        + (DateTime.Now.Year == Budget.StartingDate.Year
                                               ? PeriodBudget(Budget.StartingDate, new DateTime(DateTime.Today.Year, 12, 1))
                                               : PeriodBudget(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 1)));


        public double BudgetSoFar => TotalAllocationsSum + PeriodBudget(Budget.StartingDate, DateTime.Today);


        public double ThisMonthBudget => ThisMonthAllocationsSum + PeriodBudget(DateTime.Today.FirstDayOfMonth(), DateTime.Today.LastDayOfMonth());


        public double OverallBudgetBalance => BudgetSoFar - TotalTransactionsSum;


        public double ThisMonthBudgetBalance => ThisMonthBudget - ThisMonthTransactionsSum;


        public double LeftToEndOfYear => ThisYearBudget - Transactions.Where(x => x.TransactionDateTime.Year == DateTime.Today.Year).Sum(x => x.Amount);


        public double PeriodBudget(DateTime from, DateTime to)
        {
            var configPeriods = BudgetCategoryBudgetedAmounts.Where(x => (x.ValidTo ?? DateTime.MaxValue) >= from && x.ValidFrom <= to);
            double budget = 0;
            foreach (var config in configPeriods)
            {
                var monthsCount = OverlappingMonths(from, to, config.ValidFrom, config.ValidTo ?? DateTime.MaxValue);
                budget += monthsCount * config.MonthlyAmount;
            }

            return budget;
        }

        public double ThisMonthYetScheduledSum
        {
            get
            {
                var schedules = TransactionSchedules.Where(x => x.StartDate.Month <= DateTime.Now.Month && x.StartDate.Year <= DateTime.Now.Year);
                var today = DateTime.Today;
                double sum = 0;
                foreach (var schedule in schedules)
                {
                    var occurrences = schedule.OccurrencesInPeriod(today, new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1));
                    sum += schedule.Amount * occurrences.Count;
                }

                return sum;
            }
        }

        public double ThisYearYetScheduledSum
        {
            get
            {
                var schedules = TransactionSchedules.Where(x => x.EndDate == null || x.EndDate >= DateTime.Today);
                var today = DateTime.Today;
                double sum = 0;
                foreach (var schedule in schedules)
                {
                    var occurrences = schedule.OccurrencesInPeriod(today, new DateTime(today.Year, 12, 31));
                    sum += schedule.Amount * occurrences.Count;
                }

                return sum;
            }
        }


        private static int OverlappingMonths(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        {
            if (!(s1 <= e2 && e1 >= s2))
            {
                return 0;
            }

            var start = s1 > s2 ? s1 : s2;
            var end = e1 > e2 ? e2 : e1;

            return (12 * (end.Year - start.Year) + end.Month - start.Month) + 1;
        }

        #endregion
    }
}