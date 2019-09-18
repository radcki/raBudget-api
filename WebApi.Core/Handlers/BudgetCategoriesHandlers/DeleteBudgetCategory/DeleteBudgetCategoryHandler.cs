using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Core.Handlers.BudgetCategoriesHandlers;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory
{
    public class DeleteBudgetCategoryHandler : BaseBudgetCategoryHandler<DeleteBudgetCategoryRequest, DeleteBudgetCategoryResponse>
    {
        public DeleteBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }


        public override async Task<DeleteBudgetCategoryResponse> Handle(DeleteBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var budgetCategoryToDelete = await BudgetCategoryRepository.GetByIdAsync(request.BudgetCategoryId);
            await BudgetCategoryRepository.DeleteAsync(budgetCategoryToDelete);
            int changesCount = await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
            return new DeleteBudgetCategoryResponse()
                   {
                       ResponseType = changesCount > 0
                                          ? eResponseType.Success
                                          : eResponseType.Error,
                   };
        }
    }
}