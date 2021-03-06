using System;

namespace WebApi.Models.Dtos
{
    public class TransactionDto
    {
        #region Properties

        public int TransactionId { get; set; }
        public BudgetCategoryDto Category { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDto Budget { get; set; }
        public DateTime Date { get; set; }
        public DateTime RegisteredDate { get; set; }
        public TransactionScheduleDto TransactionSchedule { get; set; }

        #endregion
    }
}