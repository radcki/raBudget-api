using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Dto.Allocation
{
    public class AllocationFilterDto: IHaveCustomMapping
    {
        public IEnumerable<int> AllocationIdFilter { get; set; }
        public IEnumerable<int> CategoryIdFilter { get; set; }
        public IEnumerable<Guid> CreatedByUserIdFilter { get; set; }

        public DateTime? AllocationDateStartFilter { get; set; }
        public DateTime? AllocationDateEndFilter { get; set; }

        public DateTime? CreationDateStartFilter { get; set; }
        public DateTime? CreationDateEndFilter { get; set; }

        public int? LimitResults { get; set; }
        public eBudgetCategoryType? CategoryType { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<AllocationFilterDto, AllocationFilterModel>();
            configuration.CreateMap<AllocationFilterModel, AllocationFilterDto>();
        }

        #endregion
    }
}
