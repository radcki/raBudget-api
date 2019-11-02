using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory
{
    public class CreateBudgetCategoryHandler : BaseBudgetCategoryHandler<CreateBudgetCategoryRequest, BudgetCategoryDto>
    {
        private readonly IBudgetRepository _budgetRepository;

        public CreateBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IBudgetRepository budgetRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
            _budgetRepository = budgetRepository;
        }

        public override async Task<BudgetCategoryDto> Handle(CreateBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets = await _budgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            if (availableBudgets.All(s => s.Id != request.Data.BudgetId))
            {
                throw new NotFoundException("Specified budget does not exist");
            }

            var budgetCategoryEntity = Mapper.Map<BudgetCategory>(request.Data);

            for (int i = 0; i < request.Data.AmountConfigs.Count - 1; i++)
            {
                request.Data.AmountConfigs[i + 1].ValidTo = null;
                request.Data.AmountConfigs[i].ValidTo = request.Data
                                                               .AmountConfigs[i + 1].ValidFrom.AddDays(-1)
                                                               .FirstDayOfMonth();
            }

            var amountConfigs = request.Data
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