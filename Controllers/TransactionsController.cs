using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Hubs;
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
        private readonly TransactionsNotifier _transactionsNotifier;
        public TransactionsController(DataContext context, TransactionsNotifier transactionsNotifier)
        {
            DatabaseContext = context;
            _transactionsNotifier = transactionsNotifier;
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
                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});
                    var transaction = new Transaction
                                      {
                                          BudgetCategoryId = categoryEntity.BudgetCategoryId,
                                          CreatedByUserId = CurrentUser.UserId,
                                          Description = transactionDto.Description,
                                          Amount = transactionDto.Amount,
                                          CreationDateTime = DateTime.Now,
                                          TransactionDateTime = transactionDto.Date,
                                          TransactionScheduleId = transactionDto.TransactionSchedule?.TransactionScheduleId
                                      };
                    DatabaseContext.Transactions.Add(transaction);
                    DatabaseContext.SaveChanges();
                    PrecalculateTransactionsSum(transaction.BudgetCategory);
                    var savedTransactionDto = new TransactionDto
                                         {
                                             TransactionId = transaction.TransactionId,
                                             Category = new BudgetCategoryDto
                                                        {
                                                            CategoryId = transaction.BudgetCategory.BudgetCategoryId,
                                                            Icon = transaction.BudgetCategory.Icon,
                                                            Name = transaction.BudgetCategory.Name,
                                                            Type = transaction.BudgetCategory.Type
                                                        },
                                             Budget = new BudgetDto() { Id = categoryEntity.BudgetId},
                                             Date = transaction.TransactionDateTime,
                                             RegisteredDate = transaction.CreationDateTime,
                                             Description = transaction.Description,
                                             Amount = transaction.Amount
                                         };
                    _ = _transactionsNotifier.TransactionAdded(CurrentUser.Username, savedTransactionDto);
                    
                    return Ok(savedTransactionDto);
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
                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == transaction.BudgetCategory.Budget.BudgetId))
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
                                                                             .ToDtoList(),
                                               SavingCategories = transaction.BudgetCategory.Budget
                                                                             .BudgetCategories
                                                                             .Where(x => x.Type == eBudgetCategoryType.Saving)
                                                                             .ToDtoList(),
                                               SpendingCategories = transaction.BudgetCategory.Budget
                                                                               .BudgetCategories
                                                                               .Where(x => x.Type == eBudgetCategoryType.Spending)
                                                                               .ToDtoList()
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
                    var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == filters.BudgetId && x.UserId == CurrentUser.UserId);
                    if (budget.IsNullOrDefault()) return BadRequest(new {Message = "budget.invalid"});
                    
                    var transactions = DatabaseContext.Transactions
                                           .Include(b => b.BudgetCategory)
                                           .ThenInclude(b=>b.Budget)
                                           .Where(x => x.BudgetCategory.BudgetId == budget.BudgetId 
                                                       && x.BudgetCategory.Budget.UserId == budget.UserId);


                    if (!filters.StartDate.IsNullOrDefault())
                    {
                        var filter = filters.StartDate.GetValueOrDefault();
                        transactions = transactions.Where(x => x.TransactionDateTime >= filter);

                    }

                    if (!filters.EndDate.IsNullOrDefault())
                    {
                        var filter = filters.EndDate.GetValueOrDefault();
                        transactions = transactions.Where(x => x.TransactionDateTime <= filter);

                    }

                    if (!filters.Categories.IsNullOrDefault())
                    {
                        var filterIds = filters.Categories.Select(x => x.CategoryId);
                        transactions = transactions.Where(x => filterIds.Any(s=>s == x.BudgetCategory.BudgetCategoryId));
                    }


                    transactions = transactions.OrderByDescending(x => x.TransactionDateTime)
                                         .ThenByDescending(x => x.CreationDateTime);
                    var transactionsList = transactions.ToList();

                    var spendings = transactionsList.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Spending);
                    var income = transactionsList.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Income);
                    var saving = transactionsList.Where(x => x.BudgetCategory.Type == eBudgetCategoryType.Saving);

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
                    PrecalculateTransactionsSum(sourceCategory);
                    PrecalculateTransactionsSum(targetCategory);
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
                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new { Message = "category.invalid" });

                    var transactionEntity = DatabaseContext.Transactions.Single(x => x.TransactionId == id);
                    transactionEntity.Amount = transactionDto.Amount;
                    transactionEntity.Description = transactionDto.Description;
                    transactionEntity.TransactionDateTime = transactionDto.Date;
                    transactionEntity.BudgetCategoryId = categoryEntity.BudgetCategoryId;

                    DatabaseContext.SaveChanges();
                    PrecalculateTransactionsSum(categoryEntity);
                    var updatedTransactionDto = new TransactionDto()
                                                {
                                                    TransactionId = transactionEntity.TransactionId,
                                                    Category = new BudgetCategoryDto
                                                               {
                                                                   CategoryId = transactionEntity.BudgetCategory.BudgetCategoryId,
                                                                   Icon = transactionEntity.BudgetCategory.Icon,
                                                                   Name = transactionEntity.BudgetCategory.Name,
                                                                   Type = transactionEntity.BudgetCategory.Type
                                                               },
                                                    Budget = new BudgetDto() { Id = categoryEntity.BudgetId },
                                                    Date = transactionEntity.TransactionDateTime,
                                                    RegisteredDate = transactionEntity.CreationDateTime,
                                                    Description = transactionEntity.Description,
                                                    Amount = transactionEntity.Amount
                                                };
                    _ = _transactionsNotifier.TransactionUpdated(CurrentUser.Username, updatedTransactionDto);

                    return Ok(updatedTransactionDto);
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

                    if (!CurrentUser.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new { Message = "category.invalid" });

                    DatabaseContext.Transactions.Remove(transactionEntity);

                    DatabaseContext.SaveChanges();
                    PrecalculateTransactionsSum(transactionEntity.BudgetCategory);

                    _ = _transactionsNotifier.TransactionRemoved(CurrentUser.Username, id);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

            return Unauthorized();
        }

        private void PrecalculateTransactionsSum(BudgetCategory category)
        {
            DatabaseContext.BudgetCategories
                        .First(x=>x.BudgetCategoryId == category.BudgetCategoryId)
                        .TransactionsSum = category.Transactions.Sum(x => x.Amount);
            DatabaseContext.SaveChanges();
        }
    }
}