using System;
using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using raBudget.Core.Interfaces.Mapping;

namespace raBudget.Core.Dto.User
{
    public class UserDto: IHaveCustomMapping
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }
        public DateTime CreationDate { get; set; }

        #region Implementation of IHaveCustomMapping

        /// <inheritdoc />
        public void CreateMappings(Profile configuration)
        {
            // dto -> entity
            configuration.CreateMap<UserDto, Domain.Entities.User>()
                         .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.UserId));

            // entity -> dto
            configuration.CreateMap<Domain.Entities.User, UserDto>()
                         .ForMember(dto => dto.UserId, opt => opt.MapFrom(entity => entity.Id));
        }

        #endregion

    }
}
