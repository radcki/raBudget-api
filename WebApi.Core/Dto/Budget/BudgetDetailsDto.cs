using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetDetailsDto : BudgetDto, IHaveCustomMapping
    {
        #region Properties

        public bool Default { get; set; }

        public IDictionary<eBudgetCategoryType, List<BudgetCategoryDto>> BudgetCategories { get; set; }

        #endregion

        #region Methods

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public new void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetDetailsDto, Domain.Entities.Budget>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.BudgetId))
                         .ForMember(entity => entity.OwnedByUserId, opt => opt.MapFrom(dto => dto.OwnedByUser.UserId))
                         .ForMember(entity => entity.BudgetCategories, opt => opt.MapFrom(dto => dto.BudgetCategories
                                                                                                    .SelectMany(x => x.Value)
                                                                                                    .ToList()));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Budget, BudgetDetailsDto>()
                         .ForMember(dto => dto.BudgetId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion

        #endregion
    }
}