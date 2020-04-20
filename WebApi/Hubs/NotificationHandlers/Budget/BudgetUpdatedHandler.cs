using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Budget.Command;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Budget
{
    public class BudgetUpdatedHandler : INotificationHandler<UpdateBudget.Notification>
    {
        private readonly BudgetsNotifier _budgetsNotifier;

        public BudgetUpdatedHandler(BudgetsNotifier budgetsNotifier)
        {
            _budgetsNotifier = budgetsNotifier;
        }

        public Task Handle(UpdateBudget.Notification notification, CancellationToken cancellationToken)
        {
            return _budgetsNotifier.Send(eBudgetHubEvent.BudgetUpdated, notification);
        }
    }
}