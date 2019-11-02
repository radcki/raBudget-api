using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;

namespace raBudget.Core.Handlers.AllocationHandlers.GetAllocation
{
    public class GetAllocationRequest : IRequest<AllocationDto>
    {
        public int AllocationId;

        public GetAllocationRequest(int allocationId)
        {
            AllocationId = allocationId;
        }
    }

    public class GetAllocationRequestValidator : AbstractValidator<GetAllocationRequest>
    {
        public GetAllocationRequestValidator()
        {
            RuleFor(x => x.AllocationId).NotEmpty();
        }
    }
}
