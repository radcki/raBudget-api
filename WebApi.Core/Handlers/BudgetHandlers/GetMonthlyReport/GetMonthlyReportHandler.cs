using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Exceptions;
using raBudget.Core.Handlers.BudgetHandlers.GetMonthlyReport;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.BudgetHandlers.GetBudget
{
    public class GetMonthlyReportHandler : BaseBudgetHandler<GetMonthlyReportRequest, MonthlyBudgetReportDto>
    {
        public GetMonthlyReportHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<MonthlyBudgetReportDto> Handle(GetMonthlyReportRequest request, CancellationToken cancellationToken)
        {
            var availableBudgets = await BudgetRepository.ListAvailableBudgets(AuthenticationProvider.User.UserId);
            if (availableBudgets.All(s => s.Id != request.BudgetId))
            {
                throw new NotFoundException("Budget was not found");
            }

            var budgetEntity = await BudgetRepository.GetByIdAsync(request.BudgetId);
            var categoryReports = budgetEntity.BudgetCategories
                                              .Select(x => new BudgetCategoryReport(x, request.Filters.DateStartFilter, request.Filters.DateEndFilter))
                                              .ToList();
            var reportDto = new MonthlyBudgetReportDto();
            reportDto.BudgetCategoryReports = categoryReports.Select(x => new BudgetCategoryMonthlyReportDto()
                                                                          {
                                                                              BudgetCategoryId = x.Category.Id,
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
            return reportDto;
        }
    }
}