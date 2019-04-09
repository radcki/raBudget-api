using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contexts;
using WebApi.Helpers;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionSchedulesController : BaseController
    {
        public TransactionSchedulesController(DataContext context)
        {
            DatabaseContext = context;
        }

        [HttpPost("create")]
        public IActionResult CreateSchedule([FromBody] TransactionScheduleDto transactionScheduleDto)
        {
            if (User != null)
                try
                {
                    var categoryEtity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     transactionScheduleDto.BudgetCategory.CategoryId);

                    if (!CurrentUser.Budgets.Any(x => x.BudgetId == categoryEtity.Budget.BudgetId))
                    {
                        return BadRequest(new {Message = "category.invalid"});
                    }

                    if (transactionScheduleDto.Frequency > 0 && transactionScheduleDto.PeriodStep == 0)
                    {
                        return BadRequest(new {Message = "transactionSchedules.invalidPeriodStep"});
                    }


                    DatabaseContext.TransactionSchedules.Add(new TransactionSchedule()
                                                             {
                                                                 Amount = transactionScheduleDto.Amount,
                                                                 BudgetCategoryId = transactionScheduleDto.BudgetCategory.CategoryId,
                                                                 Description = transactionScheduleDto.Description,
                                                                 StartDate = transactionScheduleDto.StartDate,
                                                                 EndDate = transactionScheduleDto.EndDate,
                                                                 Frequency = transactionScheduleDto.Frequency,
                                                                 PeriodStep = transactionScheduleDto.PeriodStep,
                                                             });
                    DatabaseContext.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id, int budgetId)
        {
            if (User != null)
                try
                {
                    var transactionSchedule = DatabaseContext.TransactionSchedules.Single(x => x.TransactionScheduleId == id);
                    if (!CurrentUser.Budgets.SelectMany(x=>x.BudgetCategories).Any(x=>x.BudgetCategoryId == transactionSchedule.BudgetCategoryId))
                    {
                        return BadRequest(new {Message = "transactionsSchedule.notFound"});
                    }

                    return Ok(transactionSchedule.ToDto());
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("list")]
        public IActionResult ListTransactionSchedules([FromBody] TransactionsListFiltersDto filters)
        {
            if (User != null)
                try
                {
                    var budget = CurrentUser.Budgets.Single(x => x.BudgetId == filters.BudgetId);

                    var schedules = budget.BudgetCategories.SelectMany(x=>x.TransactionSchedules).AsEnumerable();

                    if (!filters.StartDate.IsNullOrDefault())
                    {
                        schedules = schedules.Where(x => x.StartDate >= filters.StartDate);
                    }

                    if (!filters.EndDate.IsNullOrDefault())
                    {
                        schedules = schedules.Where(x => x.EndDate <= filters.EndDate);
                    }


                    schedules = schedules.OrderByDescending(x => x.Description);

                   return Ok(schedules.AsEnumerable().Select(x=>x.ToDto()).ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("{id}/update")]
        public IActionResult UpdateSchedule(int id, [FromBody] TransactionScheduleDto scheduleDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     scheduleDto.BudgetCategory.CategoryId);
                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    var schedule = categoryEntity.TransactionSchedules.Single(x => x.TransactionScheduleId == id);
                    schedule.Description = scheduleDto.Description;
                    schedule.Amount = scheduleDto.Amount;
                    schedule.StartDate = scheduleDto.StartDate;
                    schedule.EndDate = scheduleDto.EndDate;
                    schedule.Frequency = scheduleDto.Frequency;
                    schedule.PeriodStep = scheduleDto.PeriodStep;
                    
                    DatabaseContext.SaveChanges();

                    return Ok(schedule.ToDto());
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpDelete("{id}/delete/{deleteTransactions?}")]
        public IActionResult DeleteSchedule(int id, bool deleteTransactions)
        {
            if (User != null)
                try
                {
                    var scheduleEntity = DatabaseContext.TransactionSchedules.Single(x => x.TransactionScheduleId == id);

                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId == scheduleEntity.BudgetCategoryId);

                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    if (deleteTransactions)
                    {
                        DatabaseContext.Transactions.RemoveRange(scheduleEntity.Transactions);
                    }
                    else
                    {
                        // usuń klucz
                        scheduleEntity.Transactions.ToList().ForEach(x=>x.TransactionScheduleId = null);
                    }

                    DatabaseContext.TransactionSchedules.Remove(scheduleEntity);

                    DatabaseContext.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpGet("closest-transactions")]
        public IActionResult ListClosestOccurrences(DateTime endDate, int budgetId)
        {
            if (User != null || endDate.IsNullOrDefault())
                try
                {
                    var budget = CurrentUser.Budgets.Single(x => x.BudgetId == budgetId);

                    var schedules = budget.BudgetCategories
                                       .SelectMany(x => x.TransactionSchedules)
                                       .Where(x => (x.EndDate == null || x.EndDate >= DateTime.Today) && x.StartDate <= endDate)
                                       .AsEnumerable().Select(x=>x.ToDto()).ToList();

                    var occurrences = new List<TransactionDto>();
                    foreach (var schedule in schedules)
                    {
                        var scheduleOccurrences = schedule.OccurrencesInPeriod(DateTime.Today, endDate);

                        scheduleOccurrences.ForEach(date =>
                                                    {
                                                        occurrences.Add(new TransactionDto()
                                                                       {
                                                                           Amount = schedule.Amount,
                                                                           Budget = schedule.BudgetCategory.Budget,
                                                                           Category = schedule.BudgetCategory,
                                                                           Date = date,
                                                                           Description = schedule.Description,
                                                                           TransactionSchedule = schedule
                                                                       });
                                                    });
                    }

                    return Ok(occurrences.OrderBy(x=>x.Date).ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }
    }
}