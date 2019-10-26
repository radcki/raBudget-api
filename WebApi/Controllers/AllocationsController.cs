using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Allocation;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.AllocationHandlers.CreateAllocation;
using raBudget.Core.Handlers.AllocationHandlers.DeleteAllocation;
using raBudget.Core.Handlers.AllocationHandlers.GetAllocation;
using raBudget.Core.Handlers.AllocationHandlers.ListAllocation;
using raBudget.Core.Handlers.AllocationHandlers.UpdateAllocation;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class AllocationsController : BaseController
    {

        #region Allocations CRUD

        /// <summary>
        /// Get list of allocations available for user - both owned and shared
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListAllocationsRequest(new BudgetDto() { BudgetId = budgetId }));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific allocation, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetAllocationRequest(id));
            return Ok(response);
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetFiltered([FromBody] AllocationFilterDto filters, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListAllocationsRequest(new BudgetDto() { BudgetId = budgetId })
            {
                Filters = filters
            });
            return Ok(response);
        }

        /// <summary>
        /// Create new allocation
        /// </summary>
        /// <param name="allocationDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AllocationDto allocationDto)
        {
            var response = await Mediator.Send(new CreateAllocationRequest(allocationDto));
            return Ok(response);
        }

        /// <summary>
        /// Update allocation parameters. Allocation id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] AllocationDto allocationDto, [FromRoute] int id)
        {
            allocationDto.AllocationId = id;
            var response = await Mediator.Send(new UpdateAllocationRequest(allocationDto));
            return Ok(response);
        }

        /// <summary>
        /// Delete allocation
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteAllocationRequest(id));
            return Ok(response);
        }

        #endregion
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
                    if (!allocationDto.SourceCategory.IsNullOrDefault() && allocationDto.SourceCategory.TargetBudgetCategoryId != 0)
                    {
                        var sourceCategory =
                            DatabaseContext.BudgetCategories.Single(x => x.TargetBudgetCategoryId ==
                                                                         allocationDto.SourceCategory.TargetBudgetCategoryId);

                        if (UserEntity.Budgets.All(x => x.BudgetId != sourceCategory.Budget.BudgetId))
                        {
                            return BadRequest(new {Message = "category.invalid"});
                        }

                        var subtractAllocation = new Allocation
                                                 {
                                                     TargetBudgetCategoryId = sourceCategory.TargetBudgetCategoryId,
                                                     CreatedByUserId = UserEntity.UserId,
                                                     Description = allocationDto.Description,
                                                     Amount = -allocationDto.Amount,
                                                     CreationDateTime = DateTime.Now,
                                                     AllocationDateTime = allocationDto.Date
                                                 };
                        DatabaseContext.Allocations.Add(subtractAllocation);
                    }

                    var destinationCategory =
                        DatabaseContext.BudgetCategories.Single(x => x.TargetBudgetCategoryId ==
                                                                     allocationDto.DestinationCategory.TargetBudgetCategoryId);

                    if (UserEntity.Budgets.All(x => x.BudgetId != destinationCategory.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    var allocation = new Allocation
                                     {
                                         TargetBudgetCategoryId = destinationCategory.TargetBudgetCategoryId,
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
                                                            TargetBudgetCategoryId = allocation.BudgetCategory.TargetBudgetCategoryId,
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
                                                            TargetBudgetCategoryId = allocation.BudgetCategory.TargetBudgetCategoryId,
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
                                                                TargetBudgetCategoryId = x.TargetBudgetCategoryId
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
                        DatabaseContext.BudgetCategories.Single(x => x.TargetBudgetCategoryId ==
                                                                     allocationDto.DestinationCategory.TargetBudgetCategoryId);
                    if (UserEntity.Budgets.All(x => x.BudgetId != categoryEntity.Budget.BudgetId))
                        return BadRequest(new {Message = "category.invalid"});

                    var allocationEntity = DatabaseContext.Allocations.Single(x => x.AllocationId == id);
                    allocationEntity.Amount = allocationDto.Amount;
                    allocationEntity.Description = allocationDto.Description;
                    allocationEntity.AllocationDateTime = allocationDto.Date;
                    allocationEntity.TargetBudgetCategoryId = categoryEntity.TargetBudgetCategoryId;

                    DatabaseContext.SaveChanges();
                    PrecalculateAllocationsSum(categoryEntity);
                    return Ok(new AllocationDto
                              {
                                  AllocationId = allocationEntity.AllocationId,
                                  DestinationCategory = new BudgetCategoryDto
                                                        {
                                                            TargetBudgetCategoryId = allocationEntity.BudgetCategory.TargetBudgetCategoryId,
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
                        DatabaseContext.BudgetCategories.Single(x => x.TargetBudgetCategoryId == allocationEntity.TargetBudgetCategoryId);

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
                        .First(x=>x.TargetBudgetCategoryId == category.TargetBudgetCategoryId)
                        .AllocationsSum = category.Allocations.Sum(x => x.Amount);
            DatabaseContext.SaveChanges();
        }
        */
    }
}