using System;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Allocation
{
    public class AllocationDto : IHaveCustomMapping
    {
        #region Properties

        public int AllocationId { get; set; }
        public int TargetBudgetCategoryId { get; set; }
        public int SourceBudgetCategoryId { get; set; }
        public eBudgetCategoryType Type { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDto Budget { get; set; }
        public UserDto CreatedByUser { get; set; }
        public DateTime AllocationDate { get; set; }
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<AllocationDto, Domain.Entities.Allocation>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.AllocationId))
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.MapFrom(dto => dto.CreatedByUser.UserId))
                         .ForMember(entity => entity.AllocationDateTime, opt => opt.MapFrom(dto => dto.AllocationDate))
                         .ForMember(entity => entity.TargetBudgetCategory, opt => opt.Ignore())
                         .ForMember(entity => entity.SourceBudgetCategory, opt => opt.Ignore())
                         .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                         .ForMember(entity => entity.TargetBudgetCategoryId, opt => opt.MapFrom(dto => dto.TargetBudgetCategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Allocation, AllocationDto>()
                         .ForMember(dto => dto.AllocationId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreatedByUser, opt => opt.MapFrom(entity => entity.CreatedByUser))
                         .ForMember(dto => dto.Type, opt => opt.MapFrom(entity => entity.TargetBudgetCategory.Type))
                         .ForMember(dto => dto.AllocationDate, opt => opt.MapFrom(entity => entity.AllocationDateTime))
                         .ForMember(dto => dto.TargetBudgetCategoryId, opt => opt.MapFrom(entity => entity.TargetBudgetCategoryId))
                         .ForMember(dto => dto.SourceBudgetCategoryId, opt => opt.MapFrom(entity => entity.SourceBudgetCategoryId));
        }

        #endregion
    }
}