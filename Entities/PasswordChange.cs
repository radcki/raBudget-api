using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class PasswordChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int PasswordChangeId { get; set; }

        [Required] public DateTime ChangeDateTime { get; set; }

        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}