using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Dto.Transaction
{
    public class TransactionScheduleFilterDto: IHaveCustomMapping
    {
        public IEnumerable<int> TransactionScheduleIdFilter { get; set; }
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }

        public DateTime? StartDateStartFilter { get; set; }
        public DateTime? StartDateEndFilter { get; set; }

        public DateTime? EndDateStartFilter { get; set; }
        public DateTime? EndDateEndFilter { get; set; }

        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }

        public int? LimitResults { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }
        public eDataOrder DataOrder { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<TransactionScheduleFilterDto, TransactionScheduleFilterModel>();
            configuration.CreateMap<TransactionScheduleFilterModel, TransactionScheduleFilterDto>();
        }

        #endregion
    }
}
