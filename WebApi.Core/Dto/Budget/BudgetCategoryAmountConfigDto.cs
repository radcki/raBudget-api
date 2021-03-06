﻿using System;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Entities;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetCategoryAmountConfigDto : IHaveCustomMapping
    {
        #region Properties

        public int BudgetCategoryAmountConfigId { get; set; }
        public double MonthlyAmount { get; set; }
        public int BudgetCategoryId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        #endregion

        #region Methods

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetCategoryAmountConfigDto, BudgetCategoryBudgetedAmount>()
                         .ForMember(entity => entity.MonthlyAmount, opt => opt.MapFrom(dto => dto.MonthlyAmount))
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetCategoryAmountConfigId));

            // entity -> dto
            configuration.CreateMap<BudgetCategoryBudgetedAmount, BudgetCategoryAmountConfigDto>()
                         .ForMember(dto => dto.MonthlyAmount, opt => opt.MapFrom(entity => entity.MonthlyAmount))
                         .ForMember(dto => dto.BudgetCategoryAmountConfigId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion

        #endregion
    }
}