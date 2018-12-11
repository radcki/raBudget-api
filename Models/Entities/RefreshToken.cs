using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models.Entities
{
    public class RefreshToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int RefreshTokenId { get; set; }

        [Required] public string Token { get; set; }

        [Required] public string ClientId { get; set; }

        [Required] public DateTime ValidTo { get; set; }

        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}