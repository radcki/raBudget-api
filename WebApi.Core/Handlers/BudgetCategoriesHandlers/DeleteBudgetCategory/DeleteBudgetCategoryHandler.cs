using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory
{
    public class DeleteBudgetCategoryHandler : BaseBudgetCategoryHandler<DeleteBudgetCategoryRequest, Unit>
    {
        public DeleteBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }


        public override async Task<Unit> Handle(DeleteBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var isAccessible = await BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetCategoryId);
            if (!isAccessible) {
                throw new NotFoundException("Specified budget category does not exist");
            }

            var budgetCategoryToDelete = await BudgetCategoryRepository.GetByIdAsync(request.BudgetCategoryId);
            await BudgetCategoryRepository.DeleteAsync(budgetCategoryToDelete);
            await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
            return new Unit();
        }
    }
}