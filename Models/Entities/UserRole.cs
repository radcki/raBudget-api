using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.Enum;

namespace WebApi.Models.Entities
{
    public class UserRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int UserRoleId { get; set; }

        [Required] public eRole Role { get; set; }

        /*
         * Klucze obce 
         */
        [Required] public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}