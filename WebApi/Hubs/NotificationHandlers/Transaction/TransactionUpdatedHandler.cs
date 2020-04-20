using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Transaction
{
    public class TransactionUpdatedHandler : INotificationHandler<UpdateTransaction.Notification>
    {
        private readonly TransactionsNotifier _transactionsNotifier;

        public TransactionUpdatedHandler(TransactionsNotifier transactionsNotifier)
        {
            _transactionsNotifier = transactionsNotifier;
        }

        public Task Handle(UpdateTransaction.Notification notification, CancellationToken cancellationToken)
        {
            return _transactionsNotifier.Send(eTransactionHubEvent.TransactionUpdated, notification);
        }
    }
}