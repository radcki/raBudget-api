using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class BudgetsController : BaseController
    {
        private readonly ILogger Logger;
        private readonly ILoggerFactory _loggerFactory;

        public BudgetsController(DataContext context, ILoggerFactory loggerFactory)
        {
            DatabaseContext = context;
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger("BudgetsController");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {

                if (CurrentUser.Budgets == null || !CurrentUser.Budgets.Any())
                {
                    return NotFound();
                }

                var budgets = DatabaseContext.Budgets.Where(x => x.UserId == CurrentUser.UserId)
                                          .Include(b => b.BudgetCategories).ThenInclude(b => b.BudgetCategoryAmountConfigs)
                                          .Include(b => b.BudgetCategories).ThenInclude(b => b.Transactions);
                var budgetDtos = budgets.Select(budget => budget.ToDto()).ToList();

                return Ok(budgetDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (User != null)
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                    var budget = userEntity.Budgets.Single(x => x.BudgetId == id);

                    var balanceHandler = new BalanceHandler(budget);
                    var budgetDto = new BudgetDto
                                    {
                                        Id = budget.BudgetId,
                                        Name = budget.Name,
                                        Currency = budget.Currency,
                                        StartingDate = budget.StartingDate,
                                        Balance = balanceHandler.CurrentFunds(),
                                        Default = budget.BudgetId == CurrentUser.DefaultBudgetId,

                                        IncomeCategories = budget
                                                       .BudgetCategories
                                                       .Where(x => x.Type == eBudgetCategoryType.Income)
                                                       .AsEnumerable()
                                                       .Select(x => x.ToDto())
                                                       .ToList(),

                                        SavingCategories = budget
                                                       .BudgetCategories
                                                       .Where(x => x.Type == eBudgetCategoryType.Saving)
                                                       .AsEnumerable()
                                                       .Select(x => x.ToDto())
                                                       .ToList(),

                                        SpendingCategories = budget
                                                         .BudgetCategories
                                                         .Where(x => x.Type == eBudgetCategoryType.Spending)
                                                         .AsEnumerable()
                                                         .Select(x => x.ToDto())
                                                         .ToList()
                                    };
                    return Ok(budgetDto);
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpPut("create")]
        public IActionResult Create([FromBody] BudgetDto budgetDto)
        {
            try
            {
                // dodanie budżetu
                var budgetEntity = new Budget
                                   {
                                       Name = budgetDto.Name,
                                       Currency = budgetDto.Currency,
                                       StartingDate = budgetDto.StartingDate.FirstDayOfMonth(),
                                       UserId = CurrentUser.UserId
                                   };

                DatabaseContext.Add(budgetEntity);
                DatabaseContext.SaveChanges();

                // ustaw domyślny budżet jeżeli jest tylko jeden
                if (CurrentUser.Budgets.Count() == 1) CurrentUser.DefaultBudgetId = budgetEntity.BudgetId;

                // dodanie kategorii budżetowych
                var categoryEntities = new List<BudgetCategory>();
                categoryEntities.AddRange(budgetDto
                                      .IncomeCategories
                                      .Select(x => new BudgetCategory
                                                   {
                                                       BudgetId = budgetEntity.BudgetId,
                                                       Icon = x.Icon,
                                                       BudgetCategoryAmountConfigs = new List<BudgetCategoryAmountConfig>
                                                                                     {
                                                                                         new BudgetCategoryAmountConfig
                                                                                         {
                                                                                             ValidFrom = budgetDto.StartingDate.FirstDayOfMonth(),
                                                                                             MonthlyAmount = x.Amount
                                                                                         }
                                                                                     },
                                                       Name = x.Name,
                                                       Type = eBudgetCategoryType.Income
                                                   }));

                categoryEntities.AddRange(budgetDto
                                      .SpendingCategories
                                      .Select(x => new BudgetCategory
                                                   {
                                                       BudgetId = budgetEntity.BudgetId,
                                                       Icon = x.Icon,
                                                       BudgetCategoryAmountConfigs = new List<BudgetCategoryAmountConfig>
                                                                                     {
                                                                                         new BudgetCategoryAmountConfig
                                                                                         {
                                                                                             ValidFrom = budgetDto.StartingDate.FirstDayOfMonth(),
                                                                                             MonthlyAmount = x.Amount
                                                                                         }
                                                                                     },
                                                       Name = x.Name,
                                                       Type = eBudgetCategoryType.Spending
                                                   }));

                categoryEntities.AddRange(budgetDto
                                      .SavingCategories
                                      .Select(x => new BudgetCategory
                                                   {
                                                       BudgetId = budgetEntity.BudgetId,
                                                       Icon = x.Icon,
                                                       BudgetCategoryAmountConfigs = new List<BudgetCategoryAmountConfig>
                                                                                     {
                                                                                         new BudgetCategoryAmountConfig
                                                                                         {
                                                                                             ValidFrom = budgetDto.StartingDate.FirstDayOfMonth(),
                                                                                             MonthlyAmount = x.Amount
                                                                                         }
                                                                                     },
                                                       Name = x.Name,
                                                       Type = eBudgetCategoryType.Saving
                                                   }));

                DatabaseContext.BudgetCategories.AddRange(categoryEntities);
                DatabaseContext.SaveChanges();
                return Ok(new {budgetEntity.BudgetId});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost("{id}/setDefault")]
        public IActionResult SetDefault(int id)
        {
            try
            {
                if (CurrentUser.Budgets.All(x => x.BudgetId != id))
                    return BadRequest(new {message = "budgets.notFound"});

                var userEntity = DatabaseContext.Users.First(x => x.UserId == CurrentUser.UserId);
                userEntity.DefaultBudgetId = id;

                DatabaseContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost("{id}/update")]
        public IActionResult UpdateBudget(int id, [FromBody] BudgetDataDto budgetDataDto)
        {
            try
            {
                if (CurrentUser.Budgets.All(x => x.BudgetId != id))
                    return BadRequest(new {message = "budgets.notFound"});

                var budgetEntity = DatabaseContext.Budgets.Single(x => x.BudgetId == id);
                var categoryConfigPeriods = budgetEntity.BudgetCategories.SelectMany(x => x.BudgetCategoryAmountConfigs).Where(x => x.ValidFrom < budgetDataDto.StartingDate || x.ValidFrom == budgetEntity.StartingDate);
                foreach (var categoryConfig in categoryConfigPeriods)
                {
                    categoryConfig.ValidFrom = budgetDataDto.StartingDate.FirstDayOfMonth();
                }

                budgetEntity.Name = budgetDataDto.Name;
                budgetEntity.Currency = budgetDataDto.Currency;
                budgetEntity.StartingDate = budgetDataDto.StartingDate.FirstDayOfMonth();
                DatabaseContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpDelete("{id}/delete")]
        public IActionResult DeleteBudget(int id)
        {
            try
            {
                if (CurrentUser.Budgets.All(x => x.BudgetId != id))
                    return BadRequest(new {message = "budgets.notFound"});

                var budgetEntity = DatabaseContext.Budgets.Single(x => x.BudgetId == id);
                DatabaseContext.Budgets.Remove(budgetEntity);
                DatabaseContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }


        [HttpPost("{id}/savecategory")]
        public IActionResult SaveBudgetCategory(int id, [FromBody] BudgetCategoryDto budgetCategoryDto)
        {
            try
            {
                if (!CurrentUser.Budgets.Any(x => x.BudgetId == id))
                    return BadRequest(new {message = "budgets.notFound"});

                if (budgetCategoryDto.AmountConfigs.Count > 1)
                {
                    budgetCategoryDto.AmountConfigs = budgetCategoryDto.AmountConfigs.OrderBy(x => x.ValidFrom).ToList();

                    for (int i = 0; i < budgetCategoryDto.AmountConfigs.Count - 1; i++)
                    {
                        budgetCategoryDto.AmountConfigs[i + 1].ValidTo = null;
                        budgetCategoryDto.AmountConfigs[i].ValidTo = budgetCategoryDto.AmountConfigs[i + 1].ValidFrom.AddDays(-1).FirstDayOfMonth();
                    }
                }

                // usunięcie daty validto z ostatniego okresu, potrzebne dla usuwania okresu
                budgetCategoryDto.AmountConfigs.Where(x => x.ValidFrom == budgetCategoryDto.AmountConfigs.Max(m => m.ValidFrom)).FirstOrDefault().ValidTo = null;

                if (budgetCategoryDto.CategoryId == 0)
                {
                    var categoryEntity = new BudgetCategory
                                         {
                                             BudgetId = id,
                                             Name = budgetCategoryDto.Name,
                                             BudgetCategoryAmountConfigs = budgetCategoryDto.AmountConfigs.Select(s => new BudgetCategoryAmountConfig
                                                                                                                       {
                                                                                                                           MonthlyAmount = s.Amount,
                                                                                                                           ValidFrom = s.ValidFrom,
                                                                                                                           ValidTo = s.ValidTo
                                                                                                                       }).ToList(),
                                             Icon = budgetCategoryDto.Icon ?? "",
                                             Type = budgetCategoryDto.Type
                                         };
                    DatabaseContext.Add(categoryEntity);
                    DatabaseContext.SaveChanges();
                    budgetCategoryDto.CategoryId = categoryEntity.BudgetCategoryId;
                    return Ok(budgetCategoryDto);
                }
                else
                {
                    if (!DatabaseContext.BudgetCategories.Any(x => x.BudgetCategoryId == budgetCategoryDto.CategoryId))
                        return BadRequest(new {message = "categories.notFound"});

                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     budgetCategoryDto.CategoryId);
                    categoryEntity.Name = budgetCategoryDto.Name;
                    categoryEntity.Icon = budgetCategoryDto.Icon ?? "";
                    DatabaseContext.BudgetCategoryAmountConfigs.RemoveRange(categoryEntity.BudgetCategoryAmountConfigs);
                    categoryEntity.BudgetCategoryAmountConfigs = budgetCategoryDto.AmountConfigs.Select(s => new BudgetCategoryAmountConfig
                                                                                                             {
                                                                                                                 MonthlyAmount = s.Amount,
                                                                                                                 ValidFrom = s.ValidFrom,
                                                                                                                 ValidTo = s.ValidTo
                                                                                                             }).ToList();
                    DatabaseContext.SaveChanges();
                    budgetCategoryDto.Type = categoryEntity.Type;
                    return Ok(budgetCategoryDto);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warning, "Exception during category save: " + ex + "; " + (ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
                return BadRequest(new {message = ex.InnerException.Message ?? ex.Message});
            }
        }

        [HttpDelete("{budgetId}/deletecategory/{categoryId}")]
        public IActionResult DeleteBudgetCategory(int budgetId, int categoryId)
        {
            try
            {
                if (!CurrentUser.Budgets.Any(x => x.BudgetId == budgetId))
                    return BadRequest(new {message = "budgets.notFound"});

                if (!DatabaseContext.BudgetCategories.Any(x => x.BudgetId == budgetId &&
                                                               x.BudgetCategoryId == categoryId))
                    return BadRequest(new {message = "categories.notFound"});

                var categoryEntity =
                    DatabaseContext.BudgetCategories.Single(x =>
                                                                x.BudgetId == budgetId &&
                                                                x.BudgetCategoryId == categoryId);
                DatabaseContext.BudgetCategories.Remove(categoryEntity);
                DatabaseContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/categoriesbalance/spending")]
        public IActionResult SpendingBalance(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == budgetId && x.UserId == userId);
                var balanceHandler = new BalanceHandler(budget, eBudgetCategoryType.Spending);

                return Ok(balanceHandler.SpendingCategoriesBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/categoriesbalance/saving")]
        public IActionResult SavingBalance(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == budgetId && x.UserId == userId);
                var balanceHandler = new BalanceHandler(budget, eBudgetCategoryType.Saving);

                return Ok(balanceHandler.SavingCategoriesBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/categoriesbalance/income")]
        public IActionResult IncomeBalance(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == budgetId && x.UserId == userId);
                var balanceHandler = new BalanceHandler(budget, eBudgetCategoryType.Income);

                return Ok(balanceHandler.IncomeCategoriesBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/unassigned")]
        public IActionResult UnassignedFunds(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == budgetId && x.UserId == userId);
                var balanceHandler = new BalanceHandler(budget);

                return Ok(new {Funds = balanceHandler.UnassignedFunds()});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/report/period")]
        public IActionResult PeriodReport(int budgetId, DateTime startDate, DateTime endDate)
        {
            if (User == null) return Unauthorized();
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var reportHandler = new ReportHandler(budget, startDate, endDate);

                return Ok(new
                          {
                              Spending = reportHandler.SpendingCategoriesSummary,
                              Saving = reportHandler.SavingCategoriesSummary,
                              Income = reportHandler.IncomeCategoriesSummary
                          });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("{budgetId}/report/monthly")]
        public IActionResult MonthlyReport(int budgetId, DateTime startDate, DateTime endDate)
        {
            if (User == null) return Unauthorized();
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var reportHandler = new ReportHandler(budget, startDate, endDate);

                return Ok(new
                          {
                              Spending = reportHandler.SpendingCategoriesByMonth,
                              Saving = reportHandler.SavingCategoriesByMonth,
                              Income = reportHandler.IncomeCategoriesByMonth
                          });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}