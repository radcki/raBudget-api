using System.Linq;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetMonthlyReportRequest : IRequest<MonthlyBudgetReportDto>
    {
        public int BudgetId;
        public ReportFilterDto Filters;

        public GetMonthlyReportRequest(int budgetId, ReportFilterDto filters)
        {
            BudgetId = budgetId;
            Filters = filters;  
        }
    }

    public class GetMonthlyReportRequestValidator : AbstractValidator<GetMonthlyReportRequest>
    {
        public GetMonthlyReportRequestValidator()
        {
            RuleFor(x => x.BudgetId).NotEmpty();
        }
    }
}
