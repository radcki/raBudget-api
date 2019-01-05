using System;
using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
{
    public class BudgetCategoryAmountConfigDto
    {
        public int? BudgetCategoryAmountConfigId { get; set; }
        public double Amount { get; set; }
        public BudgetCategoryDto BudgetCategory { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}