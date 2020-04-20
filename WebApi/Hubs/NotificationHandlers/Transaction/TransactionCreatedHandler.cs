using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Transaction.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Transaction
{
    public class TransactionCreatedHandler : INotificationHandler<CreateTransaction.Notification>
    {
        private readonly TransactionsNotifier _transactionsNotifier;

        public TransactionCreatedHandler(TransactionsNotifier transactionsNotifier)
        {
            _transactionsNotifier = transactionsNotifier;
        }

        public Task Handle(CreateTransaction.Notification notification, CancellationToken cancellationToken)
        {
            return _transactionsNotifier.Send(eTransactionHubEvent.TransactionAdded, notification);
        }
    }
}