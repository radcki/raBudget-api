using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;

namespace raBudget.Core.Handlers.Budget.Query
{
    public class GetPeriodReport
    {
        public class Query : IRequest<Response>
        {
            public int BudgetId { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }

            public Query()
            {
                DateEnd = new DateTime();
            }
        }

        public class Response : BaseResponse<PeriodBudgetReportDto>
        {
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.DateEnd).NotEmpty();
                RuleFor(x => x.DateStart).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Query, Response>
        {
            public Handler(IBudgetRepository repository, IAuthenticationProvider authenticationProvider) : base(repository, null, authenticationProvider)
            {
            }

            /// <inheritdoc />
            public override async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!await BudgetRepository.IsAccessibleToUser(AuthenticationProvider.User.UserId, request.BudgetId))
                {
                    throw new NotFoundException("Budget was not found");
                }

                var budgetEntity = await BudgetRepository.GetByIdAsync(request.BudgetId);
                var categoryReports = budgetEntity.BudgetCategories
                                                  .Select(x => new BudgetCategoryReport(x, request.DateStart, request.DateEnd))
                                                  .ToList();
                var reportDto = new PeriodBudgetReportDto();
                reportDto.BudgetCategoryReports = categoryReports.Select(x => new BudgetCategoryPeriodReportDto()
                                                                              {
                                                                                  BudgetCategoryId = x.Category.Id,
                                                                                  BudgetCategoryType = x.Category.Type,
                                                                                  ReportData = new ReportDataDto()
                                                                                               {
                                                                                                   BudgetedSum = x.PeriodReport.BudgetAmount,
                                                                                                   AllocationsSum = x.PeriodReport.AllocationsSum,
                                                                                                   AveragePerDay = x.PeriodReport.AveragePerDay,
                                                                                                   TransactionsSum = x.PeriodReport.TransactionsSum
                                                                                               }
                                                                              })
                                                                 .ToList();

                reportDto.TotalPeriodReport = new ReportDataDto()
                                              {
                                                  BudgetedSum = reportDto.BudgetCategoryReports.Sum(g => g.ReportData.BudgetedSum),
                                                  AllocationsSum = reportDto.BudgetCategoryReports.Sum(g => g.ReportData.AllocationsSum),
                                                  TransactionsSum = reportDto.BudgetCategoryReports.Sum(g => g.ReportData.TransactionsSum),
                                                  AveragePerDay = reportDto.BudgetCategoryReports.Sum(g => g.ReportData.AveragePerDay)
                                              };
                return new Response() {Data = reportDto};
            }
        }
    }
}