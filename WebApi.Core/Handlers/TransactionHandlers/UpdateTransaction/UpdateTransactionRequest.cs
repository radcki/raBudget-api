﻿using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;

namespace raBudget.Core.Handlers.TransactionHandlers.UpdateTransaction
{
    public class UpdateTransactionRequest : IRequest<TransactionDetailsDto>
    {
        public TransactionDetailsDto Data;

        public UpdateTransactionRequest(TransactionDetailsDto transaction)
        {
            Data = transaction;
        }
    }


    public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
    {
        public UpdateTransactionRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.BudgetCategory).NotEmpty();
            RuleFor(x => x.Data.Amount).NotEmpty();
        }
    }
    
}
