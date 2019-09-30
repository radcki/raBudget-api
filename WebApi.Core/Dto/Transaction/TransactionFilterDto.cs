using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Dto.Transaction
{
    public class TransactionFilterDto: IHaveCustomMapping
    {
        public IEnumerable<int> TransactionIdFilter { get; set; }
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }
        public IEnumerable<int> TransactionScheduleIdFilter { get; set; }

        public DateTime? TransactionDateStartFilter { get; set; }
        public DateTime? TransactionDateEndFilter { get; set; }

        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }

        public int? LimitResults { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<TransactionFilterDto, TransactionsFilterModel>();
            configuration.CreateMap<TransactionsFilterModel, TransactionFilterDto>();
        }

        #endregion
    }
}
