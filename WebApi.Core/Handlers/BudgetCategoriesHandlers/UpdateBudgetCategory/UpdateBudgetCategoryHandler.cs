using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.Domain.ExtensionMethods;

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
            var isAccessible = BudgetCategoryRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.Data.BudgetCategoryId);

            var budgetCategoryEntity = await BudgetCategoryRepository.GetByIdAsync(request.Data.BudgetCategoryId);

            budgetCategoryEntity.Name = request.Data.Name;
            budgetCategoryEntity.Icon = request.Data.Icon;

            for (int i = 0; i < request.Data.AmountConfigs.Count - 1; i++)
            {
                request.Data.AmountConfigs[i + 1].ValidTo = null;
                request.Data.AmountConfigs[i].ValidTo = request.Data.AmountConfigs[i + 1]
                                                               .ValidFrom
                                                               .AddDays(-1)
                                                               .FirstDayOfMonth();
            }

            var amountConfigs = request.Data
                                       .AmountConfigs
                                       .Select(x => new BudgetCategoryBudgetedAmount()
                                                    {
                                                        BudgetCategoryId = budgetCategoryEntity.Id,
                                                        MonthlyAmount = x.Amount,
                                                        ValidFrom = x.ValidFrom,
                                                    })
                                       .ToList();

            budgetCategoryEntity.BudgetCategoryBudgetedAmounts = amountConfigs;

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