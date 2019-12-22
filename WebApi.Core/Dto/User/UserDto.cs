using System;
using AutoMapper;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Dto.User
{
    public class UserDto : IHaveCustomMapping
    {
        #region Properties

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }
        public DateTime CreationDate { get; set; }

        #endregion

        #region Methods

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<UserDto, Domain.Entities.User>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.UserId))
                         .ForMember(entity => entity.CreationTime, opt => opt.MapFrom(dto => dto.CreationDate.IsNullOrDefault()
                                                                                                 ? DateTime.Now
                                                                                                 : dto.CreationDate));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.User, UserDto>()
                         .ForMember(dto => dto.UserId, opt => opt.MapFrom(entity => entity.Id))
                         .ForMember(dto => dto.CreationDate, opt => opt.MapFrom(entity => entity.CreationTime));
        }

        #endregion

        #endregion
    }
}