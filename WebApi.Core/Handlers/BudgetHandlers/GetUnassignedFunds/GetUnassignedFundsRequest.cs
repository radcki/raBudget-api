using MediatR;

namespace raBudget.Core.Handlers.BudgetHandlers.GetUnassignedFunds
{
    public class GetUnassignedFundsRequest : IRequest<double>
    {
        public int BudgetId { get; set; }
        public GetUnassignedFundsRequest(int budgetId)
        {
            BudgetId = budgetId;
        }
    }
    
}
