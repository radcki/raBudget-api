using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.Transaction;
using raBudget.Core.Handlers.TransactionHandlers.CreateTransaction;
using raBudget.Core.Handlers.TransactionHandlers.DeleteTransaction;
using raBudget.Core.Handlers.TransactionHandlers.GetTransaction;
using raBudget.Core.Handlers.TransactionHandlers.ListTransactions;
using raBudget.Core.Handlers.TransactionHandlers.UpdateTransaction;
using raBudget.Core.Handlers.UserHandlers.SetDefaultBudget;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budget/{budgetId}/[controller]")]
    public class TransactionsController : BaseController
    {
        #region Transactions CRUD

        /// <summary>
        /// Get list of transactions available for user - both owned and shared
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListTransactionsRequest(new BudgetDto() {BudgetId = budgetId}));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific transaction, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetTransactionRequest(id));
            return Ok(response);
        }

        /// <summary>
        /// Create new transaction
        /// </summary>
        /// <param name="transactionDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TransactionDto transactionDto)
        {
            var response = await Mediator.Send(new CreateTransactionRequest(transactionDto));
            return Ok(response);
        }

        /// <summary>
        /// Update transaction parameters. Transaction id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] TransactionDetailsDto transactionDto, [FromRoute] int id)
        {
            transactionDto.TransactionId = id;
            var response = await Mediator.Send(new UpdateTransactionRequest(transactionDto));
            return Ok(response);
        }

        /// <summary>
        /// Delete transaction
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteTransactionRequest(id));
            return Ok(response);
        }

        #endregion


        /*
        private readonly TransactionsNotifier _transactionsNotifier;
        private readonly UserService _userService;
        private User UserEntity => _userService.GetByClaimsPrincipal(User).Data;
        public TransactionsController(DataContext context, TransactionsNotifier transactionsNotifier, UserService userService)
        {
            DatabaseContext = context;
            _transactionsNotifier = transactionsNotifier;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     transactionDto.Category.CategoryId);
                    if (UserEntity.Budgets.All(x => x.BudgetId != categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});
                    var transaction = new Transaction
                                      {
                                          BudgetCategoryId = categoryEntity.BudgetCategoryId,
                                          CreatedByUserId = User.UserId().Value,
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
                    _ = _transactionsNotifier.TransactionAdded(User.UserId().Value, savedTransactionDto);
                    
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
                    if (!_userService.GetByClaimsPrincipal(User)
                                     .Data
                                     .Budgets
                                     .Any(x=>x.BudgetId == transaction.BudgetCategory.Budget.BudgetId))
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
                    var userId = User.UserId();
                    var budget = DatabaseContext.Budgets.Single(x => x.BudgetId == filters.BudgetId && x.UserId == userId);
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
                    var budget = UserEntity.Budgets.Single(x => x.BudgetId == transactionTransferDto.BudgetId);
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

        [HttpPost("{id}")]
        public IActionResult UpdateTransaction(int id, [FromBody] TransactionDto transactionDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     transactionDto.Category.CategoryId);
                    if (!UserEntity.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
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
                    _ = _transactionsNotifier.TransactionUpdated(User.UserId().Value, updatedTransactionDto);

                    return Ok(updatedTransactionDto);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            if (User != null)
                try
                {
                    var transactionEntity = DatabaseContext.Transactions.Single(x => x.TransactionId == id);

                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId == transactionEntity.BudgetCategoryId);

                    if (!UserEntity.Budgets.Any(x=>x.BudgetId == categoryEntity.Budget.BudgetId))
                        return BadRequest(new { Message = "category.invalid" });

                    DatabaseContext.Transactions.Remove(transactionEntity);

                    DatabaseContext.SaveChanges();
                    PrecalculateTransactionsSum(transactionEntity.BudgetCategory);

                    _ = _transactionsNotifier.TransactionRemoved(User.UserId().Value, id);
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
        */
    }
}