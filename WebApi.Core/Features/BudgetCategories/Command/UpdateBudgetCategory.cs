using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Mapping;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.BudgetCategories.Command
{
    public class UpdateBudgetCategory
    {
        public class Command : IRequest<BudgetCategoryDto>
        {
            public int BudgetCategoryId { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public List<BudgetCategoryAmountConfigDto> AmountConfigs { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.BudgetCategoryId).NotEmpty();
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
                var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, command.BudgetCategoryId);

                var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(command.BudgetCategoryId);

                budgetCategoryEntity.Name = command.Name;
                budgetCategoryEntity.Icon = command.Icon;

                for (int i = 0; i < command.AmountConfigs.Count - 1; i++)
                {
                    command.AmountConfigs[i].ValidTo = command.AmountConfigs[i + 1]
                                                              .ValidFrom
                                                              .AddDays(-1)
                                                              .FirstDayOfMonth();
                    command.AmountConfigs[i + 1].ValidTo = null;
                }

                var amountConfigs = command.AmountConfigs
                                           .Select(x => new BudgetCategoryBudgetedAmount()
                                                        {
                                                            BudgetCategoryId = budgetCategoryEntity.Id,
                                                            MonthlyAmount = x.MonthlyAmount,
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