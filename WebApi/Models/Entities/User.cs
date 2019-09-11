using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Models.Entities
{
    public class User
    {
        [Key] public Guid UserId { get; set; }
        [Required] public DateTime CreationTime { get; set; }
        public string Email { get; set; }
        public int? DefaultBudgetId { get; set; }
        public virtual List<Budget> Budgets { get; set; }
    }
}