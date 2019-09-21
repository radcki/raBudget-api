using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory
{
    public class GetBudgetCategoryHandler : BaseBudgetCategoryHandler<GetBudgetCategoryRequest, BudgetCategoryDto>
    {
        public GetBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<BudgetCategoryDto> Handle(GetBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetCategoryId);
            if (!isAccessible)
            {
                throw new NotFoundException("Specified budget does not exist");
            }

            var budgetCategory = await BudgetCategoryRepository.GetByIdAsync(request.BudgetCategoryId);

            return Mapper.Map<BudgetCategoryDto>(budgetCategory);
        }
    }
}