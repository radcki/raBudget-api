using System.Collections.Generic;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetCategoryDto : IHaveCustomMapping
    {
        #region Properties

        public int BudgetCategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<BudgetCategoryAmountConfigDto> AmountConfigs { get; set; }
        public eBudgetCategoryType Type { get; set; }
        public int BudgetId { get; set; }

        #endregion

        #region Methods

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetCategoryDto, BudgetCategory>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetCategoryId))
                         .ForMember(entity => entity.BudgetCategoryBudgetedAmounts, opt => opt.MapFrom(dto => dto.AmountConfigs));

            // entity -> dto
            configuration.CreateMap<BudgetCategory, BudgetCategoryDto>()
                         .ForMember(dto => dto.BudgetCategoryId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.AmountConfigs, opt => opt.MapFrom(entity => entity.BudgetCategoryBudgetedAmounts));
        }

        #endregion

        #endregion
    }
}