using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;

namespace raBudget.Core.Handlers.AllocationHandlers.DeleteAllocation
{
    public class DeleteAllocationRequest : IRequest<AllocationDto>
    {
        public int AllocationId;

        public DeleteAllocationRequest(int allocationId)
        {
            AllocationId = allocationId;
        }
    }

    public class DeleteAllocationRequestValidator : AbstractValidator<DeleteAllocationRequest>
    {
        public DeleteAllocationRequestValidator()
        {
            RuleFor(x => x.AllocationId).NotEmpty();
        }
    }
}
