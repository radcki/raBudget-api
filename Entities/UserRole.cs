using System.ComponentModel.DataAnnotations;
using WebApi.Enum;

namespace WebApi.Entities
{
    public class UserRole
    {
        [Key] public int UserRoleId { get; set; }

        [Required] public eRole Role { get; set; }

        /*
         * Klucze obce 
         */
        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}