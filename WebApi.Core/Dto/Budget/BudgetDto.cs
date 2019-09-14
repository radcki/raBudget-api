using System;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetDto : IHaveCustomMapping
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public DateTime StartingDate { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetDto, Domain.Entities.Budget>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Budget, BudgetDto>()
                         .ForMember(dto => dto.BudgetId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion
    }
}
