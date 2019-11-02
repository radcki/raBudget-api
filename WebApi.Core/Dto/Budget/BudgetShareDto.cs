using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Entities;

namespace raBudget.Core.Dto.Budget
{
    public class BudgetShareDto : IHaveCustomMapping
    {
        #region Properties

        public BudgetDto Budget { get; set; }
        public UserDto AllowedUser { get; set; }

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<BudgetShareDto, BudgetShare>()
                         .ForMember(entity => entity.BudgetId, opt => opt.MapFrom(dto => dto.Budget.BudgetId))
                         .ForMember(entity => entity.SharedWithUserId, opt => opt.MapFrom(dto => dto.AllowedUser.UserId))
                         .ForMember(entity => entity.SharedWithUser, opt => opt.MapFrom(dto => dto.AllowedUser));

            // entity -> dto
            configuration.CreateMap<BudgetShare, BudgetShareDto>()
                         .ForMember(dto => dto.Budget, opt => opt.MapFrom(entity => entity.Budget))
                         .ForMember(dto => dto.AllowedUser, opt => opt.MapFrom(entity => entity.SharedWithUser));
        }

        #endregion
    }
}