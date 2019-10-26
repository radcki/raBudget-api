using System.Collections.Generic;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.AllocationHandlers.ListAllocation
{
    public class ListAllocationsRequest : IRequest<IEnumerable<AllocationDto>>
    {
        public BudgetDto Budget { get; set; }
        public AllocationFilterDto Filters { get; set; }

        public ListAllocationsRequest(BudgetDto budget)
        {
            Budget = budget;
        }
    }

    public class ListAllocationRequestValidator : AbstractValidator<ListAllocationsRequest>
    {
        public ListAllocationRequestValidator()
        {
            RuleFor(x => x.Budget).NotEmpty();
            RuleFor(x => x.Budget.BudgetId).NotEmpty();
        }
    }
}
