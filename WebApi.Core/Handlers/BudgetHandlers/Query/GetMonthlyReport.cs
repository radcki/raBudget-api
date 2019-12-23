using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Common;
using raBudget.Core.Exceptions;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;

namespace raBudget.Core.Handlers.BudgetHandlers.Query
{
    public class GetMonthlyReport
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

        public class Response : BaseResponse<MonthlyBudgetReportDto>
        {
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.BudgetId).NotEmpty();
                RuleFor(x => x.DateStart).NotEmpty();
                RuleFor(x => x.DateEnd).NotEmpty();
            }
        }

        public class Handler : BaseBudgetHandler<Query, Response>
        {
            public Handler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
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
                var reportDto = new MonthlyBudgetReportDto();
                reportDto.BudgetCategoryReports = categoryReports.Select(x => new BudgetCategoryMonthlyReportDto()
                                                                              {
                                                                                  BudgetCategoryId = x.Category.Id,
                                                                                  BudgetCategoryType = x.Category.Type,
                                                                                  MonthlyReports = x.MonthByMonth
                                                                                                    .Data
                                                                                                    .Select(t => new MonthReportDto()
                                                                                                                 {
                                                                                                                     Month = new Month()
                                                                                                                             {
                                                                                                                                 MonthNumber = t.Month,
                                                                                                                                 Year = t.Year
                                                                                                                             },
                                                                                                                     ReportData = new ReportDataDto()
                                                                                                                                  {
                                                                                                                                      AllocationsSum = t.AllocationsSum,
                                                                                                                                      AveragePerDay = t.AveragePerDay,
                                                                                                                                      TransactionsSum = t.TransactionsSum
                                                                                                                                  }
                                                                                                                 })
                                                                              })
                                                                 .ToList();

                reportDto.TotalMonthlyReports = reportDto.BudgetCategoryReports
                                                         .SelectMany(x => x.MonthlyReports)
                                                         .GroupBy(x => x.Month)
                                                         .Select(t => new MonthReportDto()
                                                                      {
                                                                          Month = t.Key,
                                                                          ReportData = new ReportDataDto()
                                                                                       {
                                                                                           AllocationsSum = t.Sum(g => g.ReportData.AllocationsSum),
                                                                                           TransactionsSum = t.Sum(g => g.ReportData.TransactionsSum),
                                                                                           AveragePerDay = t.Sum(g => g.ReportData.AveragePerDay)
                                                                                       }
                                                                      });
                return new Response() {Data = reportDto};
            }
        }
    }
}