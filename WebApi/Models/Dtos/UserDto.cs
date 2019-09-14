using System;

namespace WebApi.Models.Dtos
{
    public class UserDto
    {
        #region Properties

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }
        public DateTime CreationDate { get; set; }

        #endregion
    }
}