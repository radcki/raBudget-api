using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Transaction
    {
        [Key] public int TransactionId { get; set; }

        [Required] public string Description { get; set; }

        [Required] public double Amount { get; set; }

        [Required] public DateTime TransactionDateTime { get; set; }

        [Required] public DateTime CreationDateTime { get; set; }

        [Required] public int CreatedByUserId { get; set; }

        [Required] public int BudgetCategoryId { get; set; }

        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}