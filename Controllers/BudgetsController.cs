using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Enum;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BudgetsController : BaseController
    {
        public BudgetsController(DataContext context)
        {
            DatabaseContext = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                if (CurrentUser.Budgets != null && CurrentUser.Budgets.Any())
                    return Ok(CurrentUser.Budgets
                                         .Select(budget => new BudgetDto
                                                           {
                                                               Name = budget.Name,
                                                               Id = budget.BudgetId,
                                                               Currency = budget.Currency,
                                                               Balance = BalanceHandler.CurrentFunds(budget),
                                                               Default = budget.BudgetId == CurrentUser.DefaultBudgetId
                                                           })
                                         .ToList());
                return NotFound();
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
                    return Ok(new BudgetDataDto
                              {
                                  BudgetName = budget.Name,
                                  Currency = budget.Currency,
                                  StartingDate = budget.StartingDate,
                                  Balance = balanceHandler.CurrentFunds(),
                                  Default = budget.BudgetId == CurrentUser.DefaultBudgetId,
                                  IncomeCategories = budget
                                                    .BudgetCategories
                                                    .Where(x => x.Type == eBudgetCategoryType.Income)
                                                    .Select(x => new BudgetCategoryDto
                                                                 {
                                                                     CategoryId = x.BudgetCategoryId,
                                                                     Type = x.Type,
                                                                     Name = x.Name,
                                                                     Amount = x.MonthlyAmount,
                                                                     Icon = x.Icon
                                                                 })
                                                    .ToList(),
                                  SavingCategories = budget
                                                    .BudgetCategories
                                                    .Where(x => x.Type == eBudgetCategoryType.Saving)
                                                    .Select(x => new BudgetCategoryDto
                                                                 {
                                                                     CategoryId = x.BudgetCategoryId,
                                                                     Type = x.Type,
                                                                     Name = x.Name,
                                                                     Amount = x.MonthlyAmount,
                                                                     Icon = x.Icon
                                                                 })
                                                    .ToList(),
                                  SpendingCategories = budget
                                                      .BudgetCategories
                                                      .Where(x => x.Type == eBudgetCategoryType.Spending)
                                                      .Select(x => new BudgetCategoryDto
                                                                   {
                                                                       CategoryId = x.BudgetCategoryId,
                                                                       Type = x.Type,
                                                                       Name = x.Name,
                                                                       Amount = x.MonthlyAmount,
                                                                       Icon = x.Icon
                                                                   })
                                                      .ToList()
                              });
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
        public IActionResult Create([FromBody] BudgetDataDto budgetDto)
        {
            try
            {
                // dodanie budżetu
                var budgetEntity = new Budget
                                   {
                                       Name = budgetDto.BudgetName,
                                       Currency = budgetDto.Currency,
                                       StartingDate = budgetDto.StartingDate,
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
                                                          MonthlyAmount = x.Amount,
                                                          Name = x.Name,
                                                          Type = eBudgetCategoryType.Income
                                                      }));

                categoryEntities.AddRange(budgetDto
                                         .SpendingCategories
                                         .Select(x => new BudgetCategory
                                                      {
                                                          BudgetId = budgetEntity.BudgetId,
                                                          Icon = x.Icon,
                                                          MonthlyAmount = x.Amount,
                                                          Name = x.Name,
                                                          Type = eBudgetCategoryType.Spending
                                                      }));

                categoryEntities.AddRange(budgetDto
                                         .SavingCategories
                                         .Select(x => new BudgetCategory
                                                      {
                                                          BudgetId = budgetEntity.BudgetId,
                                                          Icon = x.Icon,
                                                          MonthlyAmount = x.Amount,
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
                    return BadRequest(new { message = "budgets.notFound" });

                var userEntity = DatabaseContext.Users.First(x => x.UserId == CurrentUser.UserId);
                userEntity.DefaultBudgetId = id;

                DatabaseContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                budgetEntity.Name = budgetDataDto.BudgetName;
                budgetEntity.Currency = budgetDataDto.Currency;
                budgetEntity.StartingDate = budgetDataDto.StartingDate;
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
                if (!CurrentUser.Budgets.Where(x => x.BudgetId == id).Any())
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
                if (!CurrentUser.Budgets.Where(x => x.BudgetId == id).Any())
                    return BadRequest(new {message = "budgets.notFound"});

                if (budgetCategoryDto.CategoryId == null)
                {
                    var categoryEntity = new BudgetCategory
                                         {
                                             BudgetId = id,
                                             Name = budgetCategoryDto.Name,
                                             MonthlyAmount = budgetCategoryDto.Amount,
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
                    categoryEntity.MonthlyAmount = budgetCategoryDto.Amount;
                    DatabaseContext.SaveChanges();
                    budgetCategoryDto.Type = categoryEntity.Type;
                    return Ok(budgetCategoryDto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
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
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var balanceHandler = new BalanceHandler(budget);

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
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var balanceHandler = new BalanceHandler(budget);

                return Ok(balanceHandler.SavingCategoriesBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{budgetId}/categoriesbalance/income")]
        public IActionResult IncomeBalance(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var balanceHandler = new BalanceHandler(budget);

                return Ok(balanceHandler.IncomeCategoriesBalance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{budgetId}/unassigned")]
        public IActionResult UnassignedFunds(int budgetId)
        {
            if (User == null) return Unauthorized();

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userEntity = DatabaseContext.Users.Single(x => x.UserId == userId);
                var budget = userEntity.Budgets.Single(x => x.BudgetId == budgetId);
                var balanceHandler = new BalanceHandler(budget);

                return Ok(new {Funds = balanceHandler.UnassignedFunds()});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}