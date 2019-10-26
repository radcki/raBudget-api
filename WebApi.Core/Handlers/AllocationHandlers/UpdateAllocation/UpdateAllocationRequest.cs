using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;

namespace raBudget.Core.Handlers.AllocationHandlers.UpdateAllocation
{
    public class UpdateAllocationRequest : IRequest<AllocationDto>
    {
        public AllocationDto Data;

        public UpdateAllocationRequest(AllocationDto allocation)
        {
            Data = allocation;
        }
    }

    public class UpdateAllocationRequestValidator : AbstractValidator<UpdateAllocationRequest>
    {
        public UpdateAllocationRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.TargetBudgetCategoryId).NotEmpty();
            RuleFor(x => x.Data.Amount).NotEmpty();
        }
    }
}
