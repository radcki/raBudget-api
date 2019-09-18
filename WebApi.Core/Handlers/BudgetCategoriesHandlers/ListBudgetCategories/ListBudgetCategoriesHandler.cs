using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories
{
    public class ListBudgetCategoriesHandler : BaseBudgetCategoryHandler<ListBudgetCategoriesRequest, ListBudgetCategoriesResponse>
    {
        public ListBudgetCategoriesHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
        }

        public override async Task<ListBudgetCategoriesResponse> Handle(ListBudgetCategoriesRequest request, CancellationToken cancellationToken)
        {
            var categories = await BudgetCategoryRepository.ListWithFilter(new Budget(request.BudgetId), new BudgetCategoryFilterModel());
            return categories.Any()
                       ? new ListBudgetCategoriesResponse()
                         {
                             Data = Mapper.Map<BudgetCategoryDto>(categories),
                             ResponseType = eResponseType.Success
                         }
                       : new ListBudgetCategoriesResponse()
                         {
                             Data = new BudgetCategoryDto(),
                             ResponseType = eResponseType.NoDataFound
                         };
        }
    }
}