using System;

namespace WebApi.Models.Dtos
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public BudgetCategoryDto Category { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDataDto Budget { get; set; }
        public DateTime Date { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}