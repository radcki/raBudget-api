using System;
using System.Collections.Generic;
using System.Security.Claims;
using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}