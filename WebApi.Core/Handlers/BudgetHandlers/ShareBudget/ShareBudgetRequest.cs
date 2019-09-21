﻿using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.ShareBudget
{
    public class ShareBudgetRequest : IRequest
    {
        public BudgetShareDto Data;

        public ShareBudgetRequest(BudgetShareDto budgetShare)
        {
            Data = budgetShare;
        }
    }


    public class ShareBudgetRequestValidator : AbstractValidator<ShareBudgetRequest>
    {
        public ShareBudgetRequestValidator()
        {
            RuleFor(x => x.Data.AllowedUser).NotEmpty();
            RuleFor(x => x.Data.Budget).NotEmpty();
        }
    }
    
}
