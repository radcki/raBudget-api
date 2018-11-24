using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class PasswordReset
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int PasswordResetId { get; set; }

        [Required] public string Token { get; set; }

        [Required] public DateTime GenerationDateTime { get; set; }

        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}