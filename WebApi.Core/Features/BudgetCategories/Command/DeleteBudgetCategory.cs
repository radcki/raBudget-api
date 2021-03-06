﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.BudgetCategories.Command
{
    public class DeleteBudgetCategory
    {
        public class Command : IRequest
        {
            public int BudgetCategoryId;

            public Command(int budgetCategoryId)
            {
                BudgetCategoryId = budgetCategoryId;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
            }
        }

        public class Notification : INotification
        {
            public int BudgetId { get; set; }
        }

        public class Handler : BaseBudgetCategoryHandler<Command, Unit>
        {
            private readonly IMediator _mediator;

            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider, IMediator mediator) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
                _mediator = mediator;
            }


            public override async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(command.BudgetCategoryId);
                if (!isAccessible)
                {
                    throw new NotFoundException("Specified budget category does not exist");
                }

                var budgetCategoryToDelete = await BudgetCategoryRepository.GetByIdAsync(command.BudgetCategoryId);
                await BudgetCategoryRepository.DeleteAsync(budgetCategoryToDelete);
                await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
                _ = _mediator.Publish(new Notification()
                                      {
                                          BudgetId = budgetCategoryToDelete.BudgetId
                                      }, cancellationToken);
                return new Unit();
            }
        }
    }
}