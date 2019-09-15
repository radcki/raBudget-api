using System;
using System.Collections.Generic;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetCategoryDto : IHaveCustomMapping
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public double Amount { get; set; }
        public List<BudgetCategoryAmountConfigDto> AmountConfigs { get; set; }
        public eBudgetCategoryType Type { get; set; }
        public BudgetDto Budget { get; set; }
        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetCategoryDto, Domain.Entities.BudgetCategory>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.CategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.BudgetCategory, BudgetCategoryDto>()
                         .ForMember(dto => dto.CategoryId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion
    }
}
