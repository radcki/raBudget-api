﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.Transaction.Command
{
    public class DeleteTransaction
    {
        public class Request : IRequest<Response>
        {
            public int TransactionId { get; set; }
        }

        public class Response
        {
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.TransactionId).NotEmpty();
            }
        }

        public class Notification : INotification
        {
            public int BudgetId { get; set; }
            public int TransactionId { get; set; }
        }

        public class Handler : BaseTransactionHandler<Request, Response>
        {
            private readonly IMediator _mediator;

            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             ITransactionRepository transactionRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider,
             IMediator mediator) : base(budgetCategoryRepository, transactionRepository, mapper, authenticationProvider)
            {
                _mediator = mediator;
            }

            public override async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var transactionEntity = await TransactionRepository.GetByIdAsync(request.TransactionId);
                if (transactionEntity.IsNullOrDefault() || !await BudgetCategoryRepository.IsAccessibleToUser(transactionEntity.BudgetCategoryId))
                {
                    throw new NotFoundException("Target transaction was not found.");
                }

                await TransactionRepository.DeleteAsync(transactionEntity);
                await TransactionRepository.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = transactionEntity.BudgetCategory.BudgetId,
                                          TransactionId = transactionEntity.Id
                                      }, cancellationToken);

                return new Response();
            }
        }
    }
}