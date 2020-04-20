using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.BudgetCategories.Command
{
    public class SaveBudgetCategoryOrder
    {
        public class Command : IRequest<Unit>
        {
            public List<BudgetCategory> BudgetCategoryOrder { get; set; }
        }

        public class BudgetCategory
        {
            public int BudgetCategoryId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetCategoryOrder).NotEmpty();
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
                var availableBudgets = await BudgetCategoryRepository.ListAllAsync();
                int? budgetId = null;
                for (var index = 0; index < command.BudgetCategoryOrder.Count; index++)
                {
                    var budgetCategoryId = command.BudgetCategoryOrder[index].BudgetCategoryId;
                    var budgetCategoryEntity = availableBudgets.FirstOrDefault(x => x.Id == budgetCategoryId);
                    if (budgetCategoryEntity == null)
                    {
                        throw new NotFoundException("Budget category was not found");
                    }

                    budgetId = budgetCategoryEntity.BudgetId;
                    budgetCategoryEntity.Order = index;
                    await BudgetCategoryRepository.UpdateAsync(budgetCategoryEntity);
                }

                await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
                if (budgetId != null)
                {
                    _ = _mediator.Publish(new Notification()
                                          {
                                              BudgetId = budgetId.Value
                                          },
                                          cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}