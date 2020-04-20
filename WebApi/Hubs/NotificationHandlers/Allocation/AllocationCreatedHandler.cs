using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Allocation.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Allocation
{
    public class AllocationCreatedHandler : INotificationHandler<CreateAllocation.Notification>
    {
        private readonly AllocationsNotifier _allocationsNotifier;

        public AllocationCreatedHandler(AllocationsNotifier allocationsNotifier)
        {
            _allocationsNotifier = allocationsNotifier;
        }

        public Task Handle(CreateAllocation.Notification notification, CancellationToken cancellationToken)
        {
            return _allocationsNotifier.Send(eAllocationHubEvent.AllocationAdded, notification);
        }
    }
}