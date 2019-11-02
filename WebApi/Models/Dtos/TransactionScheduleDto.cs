using System;
using System.Collections.Generic;
using raBudget.Domain.Enum;

namespace WebApi.Models.Dtos
{
    public class TransactionScheduleDto
    {
        #region Properties

        public int TransactionScheduleId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public eFrequency Frequency { get; set; }

        public int PeriodStep { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public virtual BudgetCategoryDto BudgetCategory { get; set; }

        public virtual List<TransactionDto> Transactions { get; set; }

        #endregion
    }
}