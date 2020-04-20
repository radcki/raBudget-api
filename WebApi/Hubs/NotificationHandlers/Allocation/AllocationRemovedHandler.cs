using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Allocation.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Allocation
{
    public class AllocationRemovedHandler : INotificationHandler<DeleteAllocation.Notification>
    {
        private readonly AllocationsNotifier _allocationsNotifier;

        public AllocationRemovedHandler(AllocationsNotifier allocationsNotifier)
        {
            _allocationsNotifier = allocationsNotifier;
        }

        public Task Handle(DeleteAllocation.Notification notification, CancellationToken cancellationToken)
        {
            return _allocationsNotifier.Send(eAllocationHubEvent.AllocationRemoved, notification);
        }
    }
}