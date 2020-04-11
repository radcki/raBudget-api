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
    public class CreateBudgetCategory
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
                RuleFor(x => x.Data.BudgetId).NotEmpty();
            }
        }

        public class Handler : BaseBudgetCategoryHandler<Command, BudgetCategoryDto>
        {
            private readonly IBudgetRepository _budgetRepository;

            public Handler
            (IBudgetCategoryRepository budgetCategoryRepository,
             IBudgetRepository budgetRepository,
             IMapper mapper,
             IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
            {
                _budgetRepository = budgetRepository;
            }

            public override async Task<BudgetCategoryDto> Handle(Command command, CancellationToken cancellationToken)
            {
                var availableBudgets = await _budgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
                if (availableBudgets.All(s => s.Id != command.Data.BudgetId))
                {
                    throw new NotFoundException("Specified budget does not exist");
                }

                var budgetCategoryEntity = Mapper.Map<BudgetCategory>(command.Data);

                for (int i = 0; i < command.Data.AmountConfigs.Count - 1; i++)
                {
                    command.Data.AmountConfigs[i + 1].ValidTo = null;
                    command.Data.AmountConfigs[i].ValidTo = command.Data
                                                                   .AmountConfigs[i + 1]
                                                                   .ValidFrom.AddDays(-1)
                                                                   .FirstDayOfMonth();
                }

                var amountConfigs = command.Data
                                           .AmountConfigs
                                           .Select(x => new BudgetCategoryBudgetedAmount()
                                                        {
                                                            BudgetCategoryId = budgetCategoryEntity.Id,
                                                            MonthlyAmount = x.Amount,
                                                            ValidFrom = x.ValidFrom,
                                                        })
                                           .ToList();

                budgetCategoryEntity.BudgetCategoryBudgetedAmounts = amountConfigs;

                var savedBudgetCategory = await BudgetCategoryRepository.AddAsync(budgetCategoryEntity);

                var addedRows = await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
                if (addedRows.IsNullOrDefault())
                {
                    throw new SaveFailureException(nameof(budgetCategoryEntity), budgetCategoryEntity);
                }

                return Mapper.Map<BudgetCategoryDto>(savedBudgetCategory);
            }
        }
    }
}