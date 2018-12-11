using System;
using System.Collections.Generic;
using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? DefaultBudgetId { get; set; }
        public DateTime CreationDate { get; set; }
        public List<eRole> Roles { get; set; }
    }
}