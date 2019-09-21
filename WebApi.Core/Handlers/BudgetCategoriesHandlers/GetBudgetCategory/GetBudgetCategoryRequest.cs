﻿using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory
{
    public class GetBudgetCategoryRequest : IRequest<BudgetCategoryDto>
    {
        public int BudgetCategoryId;
        public int BudgetId;

        public GetBudgetCategoryRequest(int budgetCategoryId, int budgetId)
        {
            BudgetCategoryId = budgetCategoryId;
            BudgetId = budgetId;
        }
    }


    public class GetBudgetCategoryRequestValidator : AbstractValidator<GetBudgetCategoryRequest>
    {
        public GetBudgetCategoryRequestValidator()
        {
            RuleFor(x => x.BudgetCategoryId).NotEmpty();
            
        }
    }

}
