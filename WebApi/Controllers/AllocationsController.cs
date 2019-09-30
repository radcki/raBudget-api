using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AllocationsController : BaseController
    {
        /*
        private readonly UserService _userService;
        private User UserEntity => _userService.GetByClaimsPrincipal(User).Data;
        public AllocationsController(DataContext context, UserService userService)
        {
            DatabaseContext = context;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateAllocation([FromBody] AllocationDto allocationDto)
        {
            if (User != null)
                try
                {
                    if (!allocationDto.SourceCategory.IsNullOrDefault() && allocationDto.SourceCategory.BudgetCategoryId != 0)
                    {
                        var sourceCategory =
                            DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                         allocationDto.SourceCategory.BudgetCategoryId);

                        if (UserEntity.Budgets.All(x => x.BudgetId != sourceCategory.Budget.BudgetId))
                        {
                            return BadRequest(new {Message = "category.invalid"});
                        }

                        var subtractAllocation = new Allocation
                                                 {
                                                     BudgetCategoryId = sourceCategory.BudgetCategoryId,
                                                     CreatedByUserId = UserEntity.UserId,
                                                     Description = allocationDto.Description,
                                                     Amount = -allocationDto.Amount,
                                                     CreationDateTime = DateTime.Now,
                                                     AllocationDateTime = allocationDto.Date
                                                 };
                        DatabaseContext.Allocations.Add(subtractAllocation);
                    }

                    var destinationCategory =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     allocationDto.DestinationCategory.BudgetCategoryId);

                    if (UserEntity.Budgets.All(x => x.BudgetId != destinationCategory.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    var allocation = new Allocation
                                     {
                                         BudgetCategoryId = destinationCategory.BudgetCategoryId,
                                         CreatedByUserId = UserEntity.UserId,
                                         Description = allocationDto.Description,
                                         Amount = allocationDto.Amount,
                                         CreationDateTime = DateTime.Now,
                                         AllocationDateTime = allocationDto.Date
                                     };

                    DatabaseContext.Allocations.Add(allocation);
                    DatabaseContext.SaveChanges();
                    PrecalculateAllocationsSum(allocation.BudgetCategory);
                    return Ok(new AllocationDto
                              {
                                  AllocationId = allocation.AllocationId,
                                  DestinationCategory = new BudgetCategoryDto
                                                        {
                                                            BudgetCategoryId = allocation.BudgetCategory.BudgetCategoryId,
                                                            Icon = allocation.BudgetCategory.Icon,
                                                            Name = allocation.BudgetCategory.Name,
                                                            Type = allocation.BudgetCategory.Type
                                                        },
                                  Date = allocation.AllocationDateTime,
                                  RegisteredDate = allocation.CreationDateTime,
                                  Description = allocation.Description,
                                  Amount = allocation.Amount
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
                    var allocation = DatabaseContext.Allocations.Single(x => x.AllocationId == id);
                    if (UserEntity.Budgets.All(x => x.BudgetId != allocation.BudgetCategory.Budget.BudgetId))
                    {
                        return BadRequest(new {Message = "allocations.notFound"});
                    }

                    return Ok(new AllocationDto
                              {
                                  AllocationId = allocation.AllocationId,
                                  DestinationCategory = new BudgetCategoryDto
                                                        {
                                                            BudgetCategoryId = allocation.BudgetCategory.BudgetCategoryId,
                                                            Icon = allocation.BudgetCategory.Icon,
                                                            Name = allocation.BudgetCategory.Name,
                                                            Type = allocation.BudgetCategory.Type
                                                        },
                                  Date = allocation.AllocationDateTime,
                                  RegisteredDate = allocation.CreationDateTime,
                                  Description = allocation.Description,
                                  Amount = allocation.Amount,
                                  Budget = new BudgetDataDto()
                                           {
                                               Currency = allocation.BudgetCategory.Budget.Currency,
                                               IncomeCategories = allocation.BudgetCategory.Budget
                                                                            .BudgetCategories
                                                                            .Where(x => x.Type == eBudgetCategoryType.Income)
                                                                            .ToDtoList(),
                                               SavingCategories = allocation.BudgetCategory.Budget
                                                                            .BudgetCategories
                                                                            .Where(x => x.Type == eBudgetCategoryType.Saving)
                                                                            .ToDtoList(),
                                               SpendingCategories = allocation.BudgetCategory.Budget
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
        public IActionResult ListAllocations([FromBody] TransactionsListFiltersDto filters)
        {
            if (User != null)
                try
                {
                    var budget = UserEntity.Budgets.Single(x => x.BudgetId == filters.BudgetId);

                    if (!UserEntity.Budgets.Any(x=>x.BudgetId == budget.BudgetId)) return BadRequest(new {Message = "budget.invalid"});

                    var spendings = budget.BudgetCategories.Where(x => x.Type == eBudgetCategoryType.Spending).SelectMany(x=>x.Allocations);

                    if (!filters.StartDate.IsNullOrDefault())
                    {
                        spendings = spendings.Where(x => x.AllocationDateTime >= filters.StartDate);
                    }

                    if (!filters.EndDate.IsNullOrDefault())
                    {
                        spendings = spendings.Where(x => x.AllocationDateTime <= filters.EndDate);
                    }


                    spendings = spendings.OrderByDescending(x => x.AllocationDateTime)
                                         .ThenByDescending(x => x.CreationDateTime);

                    if (filters.GroupCount != null && filters.GroupCount != 0)
                    {
                        spendings = spendings.Take(filters.GroupCount.Value).OrderByDescending(x => x.AllocationDateTime);
                    }

                    return Ok(spendings.Select(x => new AllocationDto
                                                    {
                                                        AllocationId = x.AllocationId,
                                                        Description = x.Description,
                                                        Date = x.AllocationDateTime,
                                                        RegisteredDate = x.CreationDateTime,
                                                        Amount = x.Amount,
                                                        DestinationCategory =
                                                            new BudgetCategoryDto
                                                            {
                                                                Name = x.BudgetCategory.Name,
                                                                Icon = x.BudgetCategory.Icon,
                                                                BudgetCategoryId = x.BudgetCategoryId
                                                            }
                                                    }).ToList()
                             );
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpPost("{id}")]
        public IActionResult UpdateAllocation(int id, [FromBody] AllocationDto allocationDto)
        {
            if (User != null)
                try
                {
                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId ==
                                                                     allocationDto.DestinationCategory.BudgetCategoryId);
                    if (UserEntity.Budgets.All(x => x.BudgetId != categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    var allocationEntity = DatabaseContext.Allocations.Single(x => x.AllocationId == id);
                    allocationEntity.Amount = allocationDto.Amount;
                    allocationEntity.Description = allocationDto.Description;
                    allocationEntity.AllocationDateTime = allocationDto.Date;
                    allocationEntity.BudgetCategoryId = categoryEntity.BudgetCategoryId;

                    DatabaseContext.SaveChanges();
                    PrecalculateAllocationsSum(categoryEntity);
                    return Ok(new AllocationDto
                              {
                                  AllocationId = allocationEntity.AllocationId,
                                  DestinationCategory = new BudgetCategoryDto
                                                        {
                                                            BudgetCategoryId = allocationEntity.BudgetCategory.BudgetCategoryId,
                                                            Icon = allocationEntity.BudgetCategory.Icon,
                                                            Name = allocationEntity.BudgetCategory.Name,
                                                            Type = allocationEntity.BudgetCategory.Type
                                                        },
                                  Date = allocationEntity.AllocationDateTime,
                                  RegisteredDate = allocationEntity.CreationDateTime,
                                  Description = allocationEntity.Description,
                                  Amount = allocationEntity.Amount
                              });
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAllocation(int id)
        {
            if (User != null)
                try
                {
                    var allocationEntity = DatabaseContext.Allocations.Single(x => x.AllocationId == id);

                    var categoryEntity =
                        DatabaseContext.BudgetCategories.Single(x => x.BudgetCategoryId == allocationEntity.BudgetCategoryId);

                    if (UserEntity.Budgets.All(x => x.BudgetId != categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    DatabaseContext.Allocations.Remove(allocationEntity);

                    DatabaseContext.SaveChanges();
                    PrecalculateAllocationsSum(categoryEntity);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new {message = ex.Message});
                }

            return Unauthorized();
        }

        private void PrecalculateAllocationsSum(BudgetCategory category)
        {
            DatabaseContext.BudgetCategories
                        .First(x=>x.BudgetCategoryId == category.BudgetCategoryId)
                        .AllocationsSum = category.Allocations.Sum(x => x.Amount);
            DatabaseContext.SaveChanges();
        }
        */
    }
}