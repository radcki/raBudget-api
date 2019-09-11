using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.Enum;

namespace WebApi.Models.Entities
{
    public class BudgetCategoryAmountConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public int BudgetCategoryAmountConfigId { get; set; }

        [Required] public double MonthlyAmount { get; set; }

        [Required] public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        [Required] public int BudgetCategoryId{ get; set; }

        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}