using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Budget.Command;
using raBudget.Core.Features.BudgetCategories.Command;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Budget
{
    public class BudgetCategoryUpdatedHandler : INotificationHandler<UpdateBudgetCategory.Notification>
    {
        private readonly BudgetsNotifier _budgetsNotifier;

        public BudgetCategoryUpdatedHandler(BudgetsNotifier budgetsNotifier)
        {
            _budgetsNotifier = budgetsNotifier;
        }

        public Task Handle(UpdateBudgetCategory.Notification notification, CancellationToken cancellationToken)
        {
            return _budgetsNotifier.Send(eBudgetHubEvent.BudgetCategoryUpdated, notification);
        }
    }
}