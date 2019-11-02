using System;
using AutoMapper;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.TransactionSchedule
{
    public class TransactionScheduleDto : IHaveCustomMapping
    {
        #region Properties

        public int TransactionScheduleId { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public int BudgetCategoryId { get; set; }

        public eFrequency Frequency { get; set; }

        public int PeriodStep { get; set; }
        public UserDto CreatedByUser { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<TransactionScheduleDto, Domain.Entities.TransactionSchedule>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.TransactionScheduleId))
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.MapFrom(dto => dto.CreatedByUser.UserId))
                         .ForMember(entity => entity.StartDate, opt => opt.MapFrom(dto => dto.StartDate))
                         .ForMember(entity => entity.EndDate, opt => opt.MapFrom(dto => dto.EndDate))
                         .ForMember(entity => entity.BudgetCategory, opt => opt.Ignore())
                         .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.Ignore())
                         .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.TransactionSchedule, TransactionScheduleDto>()
                         .ForMember(dto => dto.TransactionScheduleId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreatedByUser, opt => opt.MapFrom(entity => entity.CreatedByUser))
                         .ForMember(dto => dto.StartDate, opt => opt.MapFrom(entity => entity.StartDate))
                         .ForMember(dto => dto.EndDate, opt => opt.MapFrom(entity => entity.EndDate))
                         .ForMember(dto => dto.BudgetCategoryId, opt => opt.MapFrom(entity => entity.BudgetCategoryId));
        }

        #endregion
    }
}