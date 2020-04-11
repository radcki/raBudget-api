using System;
using System.Collections.Generic;
using System.Linq;
using raBudget.Domain.Enum;

namespace raBudget.Domain.Entities
{
    public class Budget : BaseEntity<int>
    {
        #region Constructors

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

        #endregion

        #region Properties

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

        #endregion

        #region Business logic

        public int DaysFromBudgetStart => (int) (DateTime.Today - StartingDate).TotalDays;

        private IEnumerable<BudgetCategory> SpendingCategories => BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending);

        private IEnumerable<BudgetCategory> IncomeCategories => BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Income);

        private IEnumerable<BudgetCategory> SavingCategories => BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Saving);

        public IEnumerable<BudgetCategoryBalance> SpendingCategoriesBalance => SpendingCategories.Select(x => new BudgetCategoryBalance(x));
        public IEnumerable<BudgetCategoryBalance> IncomeCategoriesBalance => IncomeCategories.Select(x => new BudgetCategoryBalance(x));
        public IEnumerable<BudgetCategoryBalance> SavingCategoriesBalance => SavingCategories.Select(x => new BudgetCategoryBalance(x));

        private double _currentFunds;

        public double CurrentFunds
        {
            get
            {
                if (Math.Abs(_currentFunds - (double) default) < 0.01)
                {
                    _currentFunds = IncomeCategories.Sum(x => x.TotalTransactionsSum)
                                    - SpendingCategories.Sum(x => x.TotalTransactionsSum)
                                    - SavingCategories.Sum(x => x.TotalTransactionsSum);
                }

                return _currentFunds;
            }
        }

        public double UnassignedFunds()
        {
            var budgeted = SpendingCategories.Select(x => x.OverallBudgetBalance).Where(x => x > 0).Sum();
            return CurrentFunds - budgeted;
        }

        #endregion
    }
}