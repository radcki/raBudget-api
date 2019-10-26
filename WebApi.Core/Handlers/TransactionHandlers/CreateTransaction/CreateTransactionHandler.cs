using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Dto.TransactionSchedule;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Core.Handlers.BudgetHandlers.CreateBudget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.TransactionHandlers.CreateTransaction
{
    public class CreateTransactionHandler : BaseTransactionHandler<CreateTransactionRequest, TransactionDetailsDto>
    {
        public CreateTransactionHandler(IBudgetCategoryRepository budgetCategoryRepository,
                                        ITransactionRepository transactionRepository, 
                                        IMapper mapper, 
                                        IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
        {

        }

        public override async Task<TransactionDetailsDto> Handle(CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            if (! await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.BudgetCategoryId))
            {
                throw new NotFoundException("Target budget category was not found.");
            }

            request.Data.CreatedByUser = AuthenticationProvider.User;

            var transactionEntity = Mapper.Map<Transaction>(request.Data);
            var savedTransaction = await TransactionRepository.AddAsync(transactionEntity);

            var addedRows = await TransactionRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(transactionEntity), transactionEntity);
            }

            return Mapper.Map<TransactionDetailsDto>(savedTransaction);
        }
    }
}