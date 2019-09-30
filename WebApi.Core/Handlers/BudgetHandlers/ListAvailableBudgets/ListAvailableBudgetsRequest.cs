using System.Collections.Generic;
using MediatR;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class ListAvailableBudgetsRequest : IRequest<IEnumerable<BudgetDto>>
    {
    }
    
}
