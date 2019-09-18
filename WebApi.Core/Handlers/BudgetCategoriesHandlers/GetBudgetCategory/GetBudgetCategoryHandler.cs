using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory
{
    public class GetBudgetCategoryHandler : BaseBudgetCategoryHandler<GetBudgetCategoryRequest, GetBudgetCategoryResponse>
    {
        public GetBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<GetBudgetCategoryResponse> Handle(GetBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var budgetCategory = await BudgetCategoryRepository.GetByIdAsync(request.BudgetCategoryId);
            if (budgetCategory.IsNullOrDefault())
            {
                return new GetBudgetCategoryResponse {ResponseType = eResponseType.NoDataFound};
            }

            return new GetBudgetCategoryResponse
                   {
                       ResponseType = eResponseType.Success,
                       Data = Mapper.Map<BudgetCategoryDto>(budgetCategory)
                   };
        }
    }
}