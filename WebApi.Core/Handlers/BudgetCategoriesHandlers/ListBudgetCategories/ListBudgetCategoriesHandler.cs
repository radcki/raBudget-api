using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.FilterModels;

namespace raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories
{
    public class ListBudgetCategoriesHandler : BaseBudgetCategoryHandler<ListBudgetCategoriesRequest, IEnumerable<BudgetCategoryDto>>
    {
        private protected readonly IBudgetRepository _budgetRepository;
        public ListBudgetCategoriesHandler
        (IBudgetCategoryRepository budgetCategoryRepository,
         IBudgetRepository budgetRepository,
         IMapper mapper,
         IAuthenticationProvider authenticationProvider) : base(budgetCategoryRepository, mapper, authenticationProvider)
        {
            _budgetRepository = budgetRepository;
        }

        public override async Task<IEnumerable<BudgetCategoryDto>> Handle(ListBudgetCategoriesRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets = await _budgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            if (availableBudgets.All(x=> x.Id == request.BudgetId)) {
                throw new NotFoundException("Target budget was not found");
            }

            var categories = await BudgetCategoryRepository.ListWithFilter(new Budget(request.BudgetId), new BudgetCategoryFilterModel());
            return Mapper.Map<IEnumerable<BudgetCategoryDto>>(categories);
        }
    }
}