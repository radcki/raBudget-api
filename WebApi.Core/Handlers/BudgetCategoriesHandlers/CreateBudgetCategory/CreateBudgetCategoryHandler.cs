using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

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
            if (availableBudgets.Any(s => s.Id == request.Data.Budget.BudgetId))
            {
                throw new NotFoundException("Specified budget does not exist");
            }

            var budgetCategoryEntity = Mapper.Map<BudgetCategory>(request.Data);
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