using System.Collections.Generic;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.GetUnassignedFunds
{
    public class GetCategoryTypeBalanceRequest : IRequest<IEnumerable<BudgetCategoryBalance>>
    {
        public int BudgetId { get; set; }
        public eBudgetCategoryType BudgetCategoryType { get; set; }
        public GetCategoryTypeBalanceRequest(int budgetId, eBudgetCategoryType budgetCategoryType)
        {
            BudgetId = budgetId;
            BudgetCategoryType = budgetCategoryType;
        }
    }
    
}
