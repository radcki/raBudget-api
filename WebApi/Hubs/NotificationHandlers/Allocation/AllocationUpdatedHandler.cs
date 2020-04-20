using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Core.Features.Allocation.Command;
using WebApi.Hubs;

namespace raBudget.WebApi.Hubs.NotificationHandlers.Allocation
{
    public class AllocationUpdatedHandler : INotificationHandler<UpdateAllocation.Notification>
    {
        private readonly AllocationsNotifier _allocationsNotifier;

        public AllocationUpdatedHandler(AllocationsNotifier allocationsNotifier)
        {
            _allocationsNotifier = allocationsNotifier;
        }

        public Task Handle(UpdateAllocation.Notification notification, CancellationToken cancellationToken)
        {
            return _allocationsNotifier.Send(eAllocationHubEvent.AllocationUpdated, notification);
        }
    }
}