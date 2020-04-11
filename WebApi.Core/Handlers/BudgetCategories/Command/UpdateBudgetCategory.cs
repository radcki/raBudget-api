using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.BudgetCategories.Command
{
    public class UpdateBudgetCategory
    {
        public class Command : IRequest<BudgetCategoryDto>
        {
            public BudgetCategoryDto Data;

            public Command(BudgetCategoryDto budgetCategory)
            {
                Data = budgetCategory;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Data.Name).NotEmpty();
                RuleFor(x => x.Data.BudgetCategoryId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetCategoryHandler<Command, BudgetCategoryDto>
        {
            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
            }

            public override async Task<BudgetCategoryDto> Handle(Command command, CancellationToken cancellationToken)
            {
                var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, command.Data.BudgetCategoryId);

                var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(command.Data.BudgetCategoryId);

                budgetCategoryEntity.Name = command.Data.Name;
                budgetCategoryEntity.Icon = command.Data.Icon;

                for (int i = 0; i < command.Data.AmountConfigs.Count - 1; i++)
                {
                    command.Data.AmountConfigs[i].ValidTo = command.Data.AmountConfigs[i + 1]
                                                                   .ValidFrom
                                                                   .AddDays(-1)
                                                                   .FirstDayOfMonth();
                    command.Data.AmountConfigs[i + 1].ValidTo = null;
                }

                var amountConfigs = command.Data
                                           .AmountConfigs
                                           .Select(x => new BudgetCategoryBudgetedAmount()
                                                        {
                                                            BudgetCategoryId = budgetCategoryEntity.Id,
                                                            MonthlyAmount = x.Amount,
                                                            ValidFrom = x.ValidFrom,
                                                            ValidTo = x.ValidTo
                                                        })
                                           .ToList();

                budgetCategoryEntity.BudgetCategoryBudgetedAmounts = amountConfigs;

                if (!(isAccessible))
                {
                    throw new NotFoundException("Budget category was not found");
                }

                await BudgetCategoryRepository.UpdateAsync(budgetCategoryEntity);

                var addedRows = await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(budgetCategoryEntity), budgetCategoryEntity);
                }

                return Mapper.Map<BudgetCategoryDto>(budgetCategoryEntity);
            }
        }
    }
}