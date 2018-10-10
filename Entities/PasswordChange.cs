using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class PasswordChange
    {
        [Key] public int PasswordChangeId { get; set; }

        [Required] public DateTime ChangeDateTime { get; set; }

        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}