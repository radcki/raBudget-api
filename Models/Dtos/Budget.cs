using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Features;

namespace WebApi.Models.Dtos
{
    public class BudgetDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public bool Default { get; set; }
        public DateTime StartingDate { get; set; }

        public List<BudgetCategoryDto> SpendingCategories { get; set; }
        public List<BudgetCategoryDto> SavingCategories { get; set; }
        public List<BudgetCategoryDto> IncomeCategories { get; set; }
    }
}