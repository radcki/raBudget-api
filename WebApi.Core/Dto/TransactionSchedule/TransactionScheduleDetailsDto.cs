using System;
using System.Collections.Generic;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.TransactionSchedule
{
    public class TransactionScheduleDetailsDto : TransactionScheduleDto, IHaveCustomMapping
    {
        #region Properties

        public virtual BudgetCategoryDto BudgetCategory { get; set; }

        public virtual List<TransactionDto> Transactions { get; set; }

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            
        }

        #endregion
    }
}