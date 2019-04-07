using System.Collections.Generic;
using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
{
    public class BudgetCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public double Amount { get; set; }
        public List<BudgetCategoryAmountConfigDto> AmountConfigs { get; set; }
        public eBudgetCategoryType Type { get; set; }
        public BudgetDto Budget { get; set; }
    }
}