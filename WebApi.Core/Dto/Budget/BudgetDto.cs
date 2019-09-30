using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetDto : IHaveCustomMapping
    {
        #region Properties

        public int BudgetId { get; set; }
        public string Name { get; set; }
        public Currency Currency { get; set; }
        public DateTime StartingDate { get; set; }
        public UserDto OwnedByUser { get; set; }

        public bool Default => OwnedByUser != null && OwnedByUser.DefaultBudgetId == BudgetId;

        public double CurrentFunds { get; set; }

        public IEnumerable<BudgetCategoryDto> BudgetCategories { get; set; }

        #endregion

        #region Methods

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetDto, Domain.Entities.Budget>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetId))
                         .ForMember(entity => entity.OwnedByUserId, opt => opt.MapFrom(dto => dto.OwnedByUser.UserId))
                         .ForMember(entity => entity.CurrentFunds, opt => opt.Ignore())
                         .ForMember(entity => entity.OwnedByUser, opt => opt.Ignore())
                         .ForMember(entity => entity.BudgetCategories, opt => opt.MapFrom(dto => dto.BudgetCategories));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Budget, BudgetDto>()
                         .ForMember(dto => dto.BudgetId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CurrentFunds, opt => opt.MapFrom(entity => entity.CurrentFunds))
                         .ForMember(dto => dto.OwnedByUser, opt => opt.MapFrom(entity => entity.OwnedByUser))
                         .ForMember(dto => dto.Default, opt => opt.Ignore())
                         .ForMember(dto => dto.BudgetCategories, opt => opt.MapFrom(entity => entity.BudgetCategories));
        }

        #endregion

        #endregion
    }
}