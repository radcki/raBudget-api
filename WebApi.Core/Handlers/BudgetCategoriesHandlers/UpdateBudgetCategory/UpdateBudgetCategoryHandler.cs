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
    public class UpdateBudgetCategoryHandler : BaseBudgetCategoryHandler<UpdateBudgetCategoryRequest, BudgetCategoryDto>
    {
        public UpdateBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<BudgetCategoryDto> Handle(UpdateBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var isAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.CategoryId);

            var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(request.Data.CategoryId);

            budgetCategoryEntity.Name = request.Data.Name;
            budgetCategoryEntity.Icon = request.Data.Icon;

            if (!(await isAccessible))
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