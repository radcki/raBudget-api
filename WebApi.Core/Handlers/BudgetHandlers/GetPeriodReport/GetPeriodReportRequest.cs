using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;

namespace raBudget.Core.Handlers.BudgetHandlers.GetMonthlyReport
{
    public class GetPeriodReportRequest : IRequest<PeriodBudgetReportDto>
    {
        public int BudgetId;
        public ReportFilterDto Filters;

        public GetPeriodReportRequest(int budgetId, ReportFilterDto filters)
        {
            BudgetId = budgetId;
            Filters = filters;  
        }
    }

    public class GetPeriodReportRequestValidator : AbstractValidator<GetPeriodReportRequest>
    {
        public GetPeriodReportRequestValidator()
        {
            RuleFor(x => x.BudgetId).NotEmpty();
            RuleFor(x => x.Filters.DateStartFilter).NotEmpty();
            RuleFor(x => x.Filters.DateEndFilter).NotEmpty();
        }
    }
}
