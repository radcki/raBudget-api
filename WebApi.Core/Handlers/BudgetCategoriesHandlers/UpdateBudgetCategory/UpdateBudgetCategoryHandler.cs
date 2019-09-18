using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory
{
    public class UpdateBudgetCategoryHandler : BaseBudgetCategoryHandler<UpdateBudgetCategoryRequest, UpdateBudgetCategoryResponse>
    {
        public UpdateBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<UpdateBudgetCategoryResponse> Handle(UpdateBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(request.Data.CategoryId);

            budgetCategoryEntity.Name = request.Data.Name;
            budgetCategoryEntity.Icon = request.Data.Icon;
            await BudgetCategoryRepository.UpdateAsync(budgetCategoryEntity);

            var addedRows = await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(budgetCategoryEntity), budgetCategoryEntity);
            }

            return new UpdateBudgetCategoryResponse
                   {
                       Data = Mapper.Map<BudgetCategoryDto>(budgetCategoryEntity),
                       ResponseType = eResponseType.Success
                   };
        }
    }
}