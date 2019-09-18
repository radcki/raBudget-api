using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets
{
    public class ListAvailableBudgetsRequest : IRequest<ListAvailableBudgetsResponse>
    {
    }

    public class ListAvailableBudgetsResponse : BaseResponse<IEnumerable<BudgetDto>>
    {
    }
    
}
