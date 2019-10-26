using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;

namespace raBudget.Core.Handlers.AllocationHandlers.CreateAllocation
{
    public class CreateAllocationRequest : IRequest<AllocationDto>
    {
        public AllocationDto Data;

        public CreateAllocationRequest(AllocationDto allocation)
        {
            Data = allocation;
        }
    }

    public class CreateAllocationRequestValidator : AbstractValidator<CreateAllocationRequest>
    {
        public CreateAllocationRequestValidator()
        {
            RuleFor(x => x.Data.Description).NotEmpty();
            RuleFor(x => x.Data.TargetBudgetCategoryId).NotEmpty();
            RuleFor(x => x.Data.AllocationDate).NotEmpty();
        }
    }
}