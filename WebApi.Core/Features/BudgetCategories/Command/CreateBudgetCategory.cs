using System;
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
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Features.BudgetCategories.Command
{
    public class CreateBudgetCategory
    {
        public class Command : IRequest<BudgetCategoryDto>
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public List<AmountConfigDto> AmountConfigs { get; set; }
            public eBudgetCategoryType Type { get; set; }
            public int BudgetId { get; set; }
        }

        public class AmountConfigDto
        {
            public double MonthlyAmount { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                // dto -> entity
                configuration.CreateMap<AmountConfigDto, BudgetCategoryBudgetedAmount>()
                             .ForMember(entity => entity.MonthlyAmount, opt => opt.MapFrom(dto => dto.MonthlyAmount));

                configuration.CreateMap<Command, BudgetCategory>()
                             .ForMember(entity => entity.BudgetCategoryBudgetedAmounts, opt => opt.Ignore());
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.Type).NotEmpty();
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
                if (availableBudgets.All(s => s.Id != command.BudgetId))
                {
                    throw new NotFoundException("Specified budget does not exist");
                }

                var budgetCategoryEntity = Mapper.Map<BudgetCategory>(command);

                for (int i = 0; i < command.AmountConfigs.Count - 1; i++)
                {
                    command.AmountConfigs[i + 1].ValidTo = null;
                    command.AmountConfigs[i].ValidTo = command.AmountConfigs[i + 1]
                                                              .ValidFrom.AddDays(-1)
                                                              .FirstDayOfMonth();
                }

                var amountConfigs = command.AmountConfigs
                                           .Select(x => new BudgetCategoryBudgetedAmount()
                                                        {
                                                            MonthlyAmount = x.MonthlyAmount,
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