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
    public class GetPeriodReportHandler : BaseBudgetHandler<GetPeriodReportRequest, PeriodBudgetReportDto>
    {
        public GetPeriodReportHandler(IBudgetRepository repository, IMapper mapper, IAuthenticationProvider authenticationProvider) : base(repository, mapper, authenticationProvider)
        {
        }

        /// <inheritdoc />
        public override async Task<PeriodBudgetReportDto> Handle(GetPeriodReportRequest request, CancellationToken cancellationToken)
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
            var reportDto = new PeriodBudgetReportDto();
            reportDto.BudgetCategoryReports = categoryReports.Select(x => new BudgetCategoryPeriodReportDto()
                                                                          {
                                                                              BudgetCategoryId = x.Category.Id,
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
            return reportDto;
        }
    }
}