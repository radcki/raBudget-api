using System;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.Transaction
{
    public class TransactionDto : IHaveCustomMapping
    {
        #region Properties

        public int TransactionId { get; set; }
        public BudgetCategoryDto BudgetCategory { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public BudgetDto Budget { get; set; }
        public UserDto CreatedByUser { get; set; }
        public DateTime Date { get; set; }
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        #endregion

        #region Methods

        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<TransactionDto, Domain.Entities.Transaction>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.TransactionId))
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.MapFrom(dto => dto.CreatedByUser.UserId))
                         .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategory.CategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Transaction, TransactionDto>()
                         .ForMember(dto => dto.TransactionId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreatedByUser, opt => opt.MapFrom(entity => entity.CreatedByUser))
                         .ForMember(dto => dto.BudgetCategory, opt => opt.MapFrom(entity => entity.BudgetCategory));
        }

        #endregion
    }
}