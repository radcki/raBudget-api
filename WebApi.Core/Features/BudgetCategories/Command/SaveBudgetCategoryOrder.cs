using System.Collections.Generic;
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

        public class Handler : BaseBudgetCategoryHandler<Command, Unit>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                for (var index = 0; index < command.BudgetCategoryOrder.Count; index++)
                {
                    var budgetCategoryId = command.BudgetCategoryOrder[index].BudgetCategoryId;

                    var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, budgetCategoryId);
                    var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(budgetCategoryId);
                    if (!(isAccessible))
                    {
                        throw new NotFoundException("Budget category was not found");
                    }

                    budgetCategoryEntity.Order = index;
                    await BudgetCategoryRepository.UpdateAsync(budgetCategoryEntity);
                }

                await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}