using System;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Transaction
{
    public class TransactionDto : IHaveCustomMapping
    {
        #region Properties

        public int TransactionId { get; set; }
        public int BudgetCategoryId { get; set; }
        public int? TransactionScheduleId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDto Budget { get; set; }
        public UserDto CreatedByUser { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<TransactionDto, Domain.Entities.Transaction>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.TransactionId))
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.MapFrom(dto => dto.CreatedByUser.UserId))
                         .ForMember(entity => entity.TransactionDateTime, opt => opt.MapFrom(dto => dto.TransactionDate))
                         .ForMember(entity => entity.BudgetCategory, opt => opt.Ignore())
                         .ForMember(entity => entity.CreatedByUser, opt => opt.Ignore())
                         .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Transaction, TransactionDto>()
                         .ForMember(dto => dto.TransactionId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreatedByUser, opt => opt.MapFrom(entity => entity.CreatedByUser))
                         .ForMember(dto => dto.TransactionDate, opt => opt.MapFrom(entity => entity.TransactionDateTime))
                         .ForMember(dto => dto.BudgetCategoryId, opt => opt.MapFrom(entity => entity.BudgetCategoryId));
        }

        #endregion
    }
}