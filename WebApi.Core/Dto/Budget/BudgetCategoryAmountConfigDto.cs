using System;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetCategoryAmountConfigDto : IHaveCustomMapping
    {
        public int BudgetCategoryAmountConfigId { get; set; }
        public double Amount { get; set; }
        public BudgetCategoryDto BudgetCategory { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetCategoryAmountConfigDto, Domain.Entities.BudgetCategoryBudgetedAmount>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetCategoryAmountConfigId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.BudgetCategoryBudgetedAmount, BudgetCategoryAmountConfigDto>()
                         .ForMember(dto => dto.BudgetCategoryAmountConfigId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion
    }
}
