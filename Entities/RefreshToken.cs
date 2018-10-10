using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class RefreshToken
    {
        [Key] public int RefreshTokenId { get; set; }

        [Required] public string Token { get; set; }

        [Required] public string ClientId { get; set; }

        [Required] public DateTime ValidTo { get; set; }

        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}