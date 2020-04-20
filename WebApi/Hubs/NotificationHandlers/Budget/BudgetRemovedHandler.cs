using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Budget.Command;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Budget
{
    public class BudgetRemovedHandler : INotificationHandler<DeleteBudget.Notification>
    {
        private readonly BudgetsNotifier _budgetsNotifier;

        public BudgetRemovedHandler(BudgetsNotifier budgetsNotifier)
        {
            _budgetsNotifier = budgetsNotifier;
        }

        public Task Handle(DeleteBudget.Notification notification, CancellationToken cancellationToken)
        {
            return _budgetsNotifier.Send(eBudgetHubEvent.BudgetRemoved, notification);
        }
    }
}