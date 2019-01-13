using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Models.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int UserId { get; set; }

        [Required] [MaxLength(160)] public string Email { get; set; }

        [MaxLength(160)] public string EmailVerificationCode { get; set; }

        public bool EmailVerified { get; set; }


        [Required] [MaxLength(30)] public string Username { get; set; }

        [Required] [MaxLength(160)] public string Password { get; set; }

        [Required] public DateTime CreationTime { get; set; }

        public int? DefaultBudgetId { get; set; }

        /*
         * Klucze obce 
         */
        public virtual List<PasswordChange> PasswordChanges { get; set; }
        public virtual List<PasswordReset> PasswordResets { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; set; }
        public virtual List<Budget> Budgets { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
    }
}