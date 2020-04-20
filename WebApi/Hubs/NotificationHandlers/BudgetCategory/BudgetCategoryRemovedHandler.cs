using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Budget.Command;
using raBudget.Core.Features.BudgetCategories.Command;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Budget
{
    public class BudgetCategoryRemovedHandler : INotificationHandler<DeleteBudgetCategory.Notification>
    {
        private readonly BudgetsNotifier _budgetsNotifier;

        public BudgetCategoryRemovedHandler(BudgetsNotifier budgetsNotifier)
        {
            _budgetsNotifier = budgetsNotifier;
        }

        public Task Handle(DeleteBudgetCategory.Notification notification, CancellationToken cancellationToken)
        {
            return _budgetsNotifier.Send(eBudgetHubEvent.BudgetCategoryRemoved, notification);
        }
    }
}