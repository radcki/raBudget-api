using WebApi.Enum;

namespace WebApi.Dtos
{
    public class BudgetCategoryDto
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public double Amount { get; set; }
        public eBudgetCategoryType Type { get; set; }
    }
}