namespace WebApi.Models.Dtos
{
    public class BudgetDto
    {
        public string Name { get; set; }
        public int? Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public bool Default { get; set; }
    }
}