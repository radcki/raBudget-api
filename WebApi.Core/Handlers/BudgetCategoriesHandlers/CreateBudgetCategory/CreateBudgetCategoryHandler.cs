using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.ExtensionMethods;
using raBudget.Core.Handlers.BudgetHandlers.CreateBudget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory
{
    public class CreateBudgetCategoryHandler : BaseBudgetCategoryHandler<CreateBudgetCategoryRequest, CreateBudgetCategoryResponse>
    {
        public CreateBudgetCategoryHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<CreateBudgetCategoryResponse> Handle(CreateBudgetCategoryRequest request, CancellationToken cancellationToken)
        {
            var budgetCategoryEntity = Mapper.Map<BudgetCategory>(request.Data);
            var savedBudgetCategory = await BudgetCategoryRepository.AddAsync(budgetCategoryEntity);

            var addedRows = await BudgetCategoryRepository.SaveChangesAsync(cancellationToken);
            if (addedRows.IsNullOrDefault())
            {
                throw new SaveFailureException(nameof(budgetCategoryEntity), budgetCategoryEntity);
            }

            return new CreateBudgetCategoryResponse
                   {
                       Data = Mapper.Map<BudgetCategoryDto>(savedBudgetCategory),
                       ResponseType = eResponseType.Success
                   };
        }
    }
}