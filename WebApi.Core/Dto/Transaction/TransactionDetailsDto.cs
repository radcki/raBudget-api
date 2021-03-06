﻿using System;
using AutoMapper;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.Transaction
{
    public class TransactionDetailsDto : TransactionDto, IHaveCustomMapping
    {
        #region Properties

        public TransactionScheduleDto TransactionSchedule { get; set; }

        #endregion

        #region Methods

        public new void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<TransactionDetailsDto, Domain.Entities.Transaction>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.TransactionId))
                         .ForMember(entity => entity.CreatedByUserId, opt => opt.MapFrom(dto => dto.CreatedByUser.UserId))
                         .ForMember(entity => entity.TransactionDateTime, opt => opt.MapFrom(dto => dto.TransactionDate))
                         .ForMember(entity => entity.TransactionScheduleId, opt => opt.MapFrom(dto => dto.TransactionSchedule.TransactionScheduleId))
                         .ForMember(entity => entity.BudgetCategoryId, opt => opt.MapFrom(dto => dto.BudgetCategoryId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.Transaction, TransactionDetailsDto>()
                         .ForMember(dto => dto.TransactionId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreatedByUser, opt => opt.MapFrom(entity => entity.CreatedByUser))
                         .ForMember(dto => dto.TransactionDate, opt => opt.MapFrom(entity => entity.TransactionDateTime))
                         .ForMember(dto => dto.TransactionSchedule, opt => opt.MapFrom(entity => entity.TransactionSchedule))
                         .ForMember(dto => dto.RegisteredDate, opt => opt.MapFrom(entity => entity.CreationDateTime))
                         .ForMember(dto => dto.BudgetCategoryId, opt => opt.MapFrom(entity => entity.BudgetCategoryId));
        }

        #endregion
    }
}