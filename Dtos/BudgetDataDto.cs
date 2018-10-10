using System;
using System.Collections.Generic;

namespace WebApi.Dtos
{
    public class BudgetDataDto
    {
        public string BudgetName { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
        public bool Default { get; set; }
        public DateTime StartingDate { get; set; }
        public List<BudgetCategoryDto> SpendingCategories { get; set; }
        public List<BudgetCategoryDto> SavingCategories { get; set; }
        public List<BudgetCategoryDto> IncomeCategories { get; set; }
    }
}