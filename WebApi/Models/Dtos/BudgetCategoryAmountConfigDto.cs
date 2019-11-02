using System;

namespace WebApi.Models.Dtos
{
    public class BudgetCategoryAmountConfigDto
    {
        #region Properties

        public int BudgetCategoryAmountConfigId { get; set; }
        public double Amount { get; set; }
        public BudgetCategoryDto BudgetCategory { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        #endregion
    }
}