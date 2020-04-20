using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Transaction
{
    public class TransactionRemovedHandler : INotificationHandler<DeleteTransaction.Notification>
    {
        private readonly TransactionsNotifier _transactionsNotifier;

        public TransactionRemovedHandler(TransactionsNotifier transactionsNotifier)
        {
            _transactionsNotifier = transactionsNotifier;
        }

        public Task Handle(DeleteTransaction.Notification notification, CancellationToken cancellationToken)
        {
            return _transactionsNotifier.Send(eTransactionHubEvent.TransactionRemoved, notification);
        }
    }
}