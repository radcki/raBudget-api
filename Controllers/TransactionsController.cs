using System;
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
    public class TransactionsController : BaseController
    {
        public TransactionsController(DataContext context)
        {
            DatabaseContext = context;
        }

        [HttpPost("create")]
        public IActionResult CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     transactionDto.Category.CategoryId);
                    if (!CurrentUser.Budgets.Contains(categoryEntity.Budget))
                        return BadRequest(new {Message = "category.invalid"});
                    var transaction = new Transaction
                                      {
                                          BudgetCategoryId = categoryEntity.BudgetCategoryId,
                                          CreatedByUserId = CurrentUser.UserId,
                                          Description = transactionDto.Description,
                                          Amount = transactionDto.Amount,
                                          CreationDateTime = DateTime.Now,
                                          TransactionDateTime = transactionDto.Date
                                      };
                    DatabaseContext.Transactions.Add(transaction);
                    DatabaseContext.SaveChanges();

                    return Ok(new TransactionDto
                              {
                                  TransactionId = transaction.TransactionId,
                                  Category = new BudgetCategoryDto
                                             {
                                                 CategoryId = transaction.BudgetCategory.BudgetCategoryId,
                                                 Icon = transaction.BudgetCategory.Icon,
                                                 Name = transaction.BudgetCategory.Name,
                                                 Type = transaction.BudgetCategory.Type
                                             },
                                  Date = transaction.TransactionDateTime,
                                  RegisteredDate = transaction.CreationDateTime,
                                  Description = transaction.Description,
                                  Amount = transaction.Amount
                              });
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
                    var transaction = DatabaseContext.Transactions.Single(x => x.TransactionId == id);
                    if (!CurrentUser.Budgets.Contains(transaction.BudgetCategory.Budget))
                    {
                        return BadRequest(new {Message = "transactions.notFound"});
                    }

                    return Ok(new TransactionDto
                              {
                                  TransactionId = transaction.TransactionId,
                                  Category = new BudgetCategoryDto
                                             {
                                                 CategoryId = transaction.BudgetCategory.BudgetCategoryId,
                                                 Icon = transaction.BudgetCategory.Icon,
                                                 Name = transaction.BudgetCategory.Name,
                                                 Type = transaction.BudgetCategory.Type
                                             },
                                  Date = transaction.TransactionDateTime,
                                  RegisteredDate = transaction.CreationDateTime,
                                  Description = transaction.Description,
                                  Amount = transaction.Amount,
                                  Budget = new BudgetDataDto()
                                           {
                                               Currency = transaction.BudgetCategory.Budget.Currency,
                                               IncomeCategories = transaction.BudgetCategory.Budget
                                                                             .BudgetCategories
                                                                             .Where(x => x.Type == eBudgetCategoryType.Income)
                                                                             .AsEnumerable()
                                                                             .Select(x => x.ToDto())
                                                                             .ToList(),
                                               SavingCategories = transaction.BudgetCategory.Budget
                                                                             .BudgetCategories
                                                                             .Where(x => x.Type == eBudgetCategoryType.Saving)
                                                                             .AsEnumerable()
                                                                             .Select(x => x.ToDto())
                                                                             .ToList(),
                                               SpendingCategories = transaction.BudgetCategory.Budget
                                                                               .BudgetCategories
                                                                               .Where(x => x.Type == eBudgetCategoryType.Spending)
                                                                               .AsEnumerable()
                                                                               .Select(x => x.ToDto())
                                                                               .ToList()
                                           }
                              });
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("list")]
        public IActionResult ListTransactions([FromBody] TransactionsListFiltersDto filters)
        {
            if (User != null)
                try
                {
                    var budget = CurrentUser.Budgets.Single(x => x.BudgetId == filters.BudgetId);
                    if (!CurrentUser.Budgets.Contains(budget)) return BadRequest(new {Message = "budget.invalid"});

                    var spendings = DatabaseContext.Transactions
                                                   .Where(x => budget.BudgetCategories.Contains(x.BudgetCategory)
                                                               && x.BudgetCategory.Type == eBudgetCategoryType.Spending);

                    var income = DatabaseContext.Transactions
                                                .Where(x => budget.BudgetCategories.Contains(x.BudgetCategory)
                                                            && x.BudgetCategory.Type == eBudgetCategoryType.Income);

                    var saving = DatabaseContext.Transactions
                                                .Where(x => budget.BudgetCategories.Contains(x.BudgetCategory)
                                                            && x.BudgetCategory.Type == eBudgetCategoryType.Saving);

                    if (!filters.StartDate.IsNullOrDefault())
                    {
                        var filter = filters.StartDate.GetValueOrDefault();
                        spendings = spendings.Where(x => x.TransactionDateTime >= filter);
                        income = income.Where(x => x.TransactionDateTime >= filter);
                        saving = saving.Where(x => x.TransactionDateTime >= filter);
                    }

                    if (!filters.EndDate.IsNullOrDefault())
                    {
                        var filter = filters.EndDate.GetValueOrDefault();
                        spendings = spendings.Where(x => x.TransactionDateTime <= filter);
                        income = income.Where(x => x.TransactionDateTime <= filter);
                        saving.Where(x => x.TransactionDateTime <= filter);
                    }

                    if (!filters.Categories.IsNullOrDefault())
                    {
                        var filterIds = filters.Categories.Select(x => x.CategoryId);
                        spendings = spendings.Where(x => filterIds.Any(s=>s == x.BudgetCategory.BudgetCategoryId));
                        income = income.Where(x => filterIds.Any(s=>s == x.BudgetCategory.BudgetCategoryId));
                        saving = saving.Where(x => filterIds.Any(s=>s == x.BudgetCategory.BudgetCategoryId));
                    }


                    spendings = spendings.OrderByDescending(x => x.TransactionDateTime)
                                         .ThenByDescending(x => x.CreationDateTime);

                    income = income.OrderByDescending(x => x.TransactionDateTime)
                                   .ThenByDescending(x => x.CreationDateTime);

                    saving = saving.OrderByDescending(x => x.TransactionDateTime)
                                   .ThenByDescending(x => x.CreationDateTime);

                    if (!filters.GroupCount.IsNullOrDefault())
                    {
                        var count = filters.GroupCount.GetValueOrDefault();
                        spendings = spendings.Take(count);
                        income = income.Take(count);
                        saving = saving.Take(count);
                    }

                    return Ok(new
                              {
                                  Spendings = spendings.Select(x => new TransactionDto
                                                                    {
                                                                        TransactionId = x.TransactionId,
                                                                        Description = x.Description,
                                                                        Date = x.TransactionDateTime,
                                                                        RegisteredDate = x.CreationDateTime,
                                                                        Amount = x.Amount,
                                                                        Category =
                                                                            new BudgetCategoryDto
                                                                            {
                                                                                Name = x.BudgetCategory.Name,
                                                                                Icon = x.BudgetCategory.Icon,
                                                                                CategoryId = x.BudgetCategoryId
                                                                            }
                                                                    }).ToList(),

                                  Incomes = income.Select(x => new TransactionDto
                                                               {
                                                                   TransactionId = x.TransactionId,
                                                                   Description = x.Description,
                                                                   Date = x.TransactionDateTime,
                                                                   RegisteredDate = x.CreationDateTime,
                                                                   Amount = x.Amount,
                                                                   Category = new BudgetCategoryDto
                                                                              {
                                                                                  Name = x.BudgetCategory.Name,
                                                                                  Icon = x.BudgetCategory.Icon,
                                                                                  CategoryId = x.BudgetCategoryId
                                                                              }
                                                               }).ToList(),

                                  Savings = saving.Select(x => new TransactionDto
                                                               {
                                                                   TransactionId = x.TransactionId,
                                                                   Description = x.Description,
                                                                   Date = x.TransactionDateTime,
                                                                   RegisteredDate = x.CreationDateTime,
                                                                   Amount = x.Amount,
                                                                   Category = new BudgetCategoryDto
                                                                              {
                                                                                  Name = x.BudgetCategory.Name,
                                                                                  Icon = x.BudgetCategory.Icon,
                                                                                  CategoryId = x.BudgetCategoryId
                                                                              }
                                                               }).ToList()
                              });
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("transfer")]
        public IActionResult TransferToCategory([FromBody] TransactionsTransferDto transactionTransferDto)
        {
            if (User != null)
                try
                {
                    var budget = CurrentUser.Budgets.Single(x => x.BudgetId == transactionTransferDto.BudgetId);
                    var sourceCategory =
                        budget.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                            transactionTransferDto.SourceCategory);
                    var targetCategory =
                        budget.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                            transactionTransferDto.TargetCategory);

                    sourceCategory.Transactions.ForEach(x => x.BudgetCategoryId = targetCategory.BudgetCategoryId);

                    DatabaseContext.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("{id}/update")]
        public IActionResult UpdateTransaction(int id, [FromBody] TransactionDto transactionDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     transactionDto.Category.CategoryId);
                    if (!CurrentUser.Budgets.Contains(categoryEntity.Budget))
                        return BadRequest(new { Message = "category.invalid" });

                    var transactionEntity = DatabaseContext.Transactions.Single(x => x.TransactionId == id);
                    transactionEntity.Amount = transactionDto.Amount;
                    transactionEntity.Description = transactionDto.Description;
                    transactionEntity.TransactionDateTime = transactionDto.Date;
                    transactionEntity.BudgetCategoryId = categoryEntity.BudgetCategoryId;

                    DatabaseContext.SaveChanges();

                    return Ok(new TransactionDto
                    {
                        TransactionId = transactionEntity.TransactionId,
                        Category = new BudgetCategoryDto
                        {
                            CategoryId = transactionEntity.BudgetCategory.BudgetCategoryId,
                            Icon = transactionEntity.BudgetCategory.Icon,
                            Name = transactionEntity.BudgetCategory.Name,
                            Type = transactionEntity.BudgetCategory.Type
                        },
                        Date = transactionEntity.TransactionDateTime,
                        RegisteredDate = transactionEntity.CreationDateTime,
                        Description = transactionEntity.Description,
                        Amount = transactionEntity.Amount
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

            return Unauthorized();
        }

        [HttpDelete("{id}/delete")]
        public IActionResult DeleteTransaction(int id)
        {
            if (User != null)
                try
                {
                    var transactionEntity = DatabaseContext.Transactions.Single(x => x.TransactionId == id);

                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId == transactionEntity.BudgetCategoryId);

                    if (!CurrentUser.Budgets.Contains(categoryEntity.Budget))
                        return BadRequest(new { Message = "category.invalid" });

                    DatabaseContext.Transactions.Remove(transactionEntity);

                    DatabaseContext.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

            return Unauthorized();
        }
    }
}