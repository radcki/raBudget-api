using System;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.TransactionSchedule
{
    public class TransactionScheduleDto : IHaveCustomMapping
    {
        #region Properties

        public int TransactionScheduleId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public eFrequency Frequency { get; set; }

        public int PeriodStep { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            
        }

        #endregion
    }
}