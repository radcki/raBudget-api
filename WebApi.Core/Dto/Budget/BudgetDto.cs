using System;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Entities;

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
                         .ForMember(entity => entity.CurrencyCode, opt => opt.MapFrom(dto => dto.Currency.CurrencyCode));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Budget, BudgetDto>()
                         .ForMember(dto => dto.BudgetId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.Currency, opt => opt.MapFrom(entity => entity.Currency));
        }

        #endregion

        #endregion
    }
}