using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.CreateBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.DeleteBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.GetBudgetCategory;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.ListBudgetCategories;
using raBudget.Core.Handlers.BudgetCategoriesHandlers.UpdateBudgetCategory;
using raBudget.Core.Handlers.BudgetHandlers.CreateBudget;
using raBudget.Core.Handlers.BudgetHandlers.GetBudget;
using raBudget.Core.Handlers.BudgetHandlers.ListAvailableBudgets;
using raBudget.Domain.Enum;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/budgets/{budgetId}/[controller]")]
    public class BudgetCategoriesController : BaseController
    {
        /// <summary>
        /// Get list of budget categories belonging to budget
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetCategoryDto>>> Get([FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new ListBudgetCategoriesRequest(budgetId));
            return Ok(response);
        }

        /// <summary>
        ///  Get details of specific budget category, identified by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> GetById([FromRoute] int id, [FromRoute] int budgetId)
        {
            var response = await Mediator.Send(new GetBudgetCategoryRequest(id, budgetId));
            return Ok(response);
        }

        /// <summary>
        /// Create new budget
        /// </summary>
        /// <param name="budgetCategoryDto"></param>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BudgetCategoryDto>> Create([FromBody] BudgetCategoryDto budgetCategoryDto, [FromRoute] int budgetId)
        {
            budgetCategoryDto.BudgetId = budgetId;
            var response = await Mediator.Send(new CreateBudgetCategoryRequest(budgetCategoryDto));
            return Ok();
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BudgetCategoryDto>> Update([FromBody] BudgetCategoryDto budgetCategoryDto, [FromRoute] int budgetId)
        {
            budgetCategoryDto.BudgetId = budgetId;

            var response = await Mediator.Send(new UpdateBudgetCategoryRequest(budgetCategoryDto));
            return Ok(response);
        }

        /// <summary>
        /// Update budget parameters. Budget id in request body will be ignored
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, int budgetId)
        {
            var response = await Mediator.Send(new DeleteBudgetCategoryRequest(id));
            return Ok(response);
        }


        /*
        
        [HttpPost("{id}/categories")]
        public IActionResult CreateBudgetCategory(int id, [FromBody] BudgetCategoryDto budgetCategoryDto)
        {
            try
            {
                if (UserEntity.Budgets.All(x => x.BudgetId != id))
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

                var categoryEntity = new BudgetCategory
                                     {
                                         BudgetId = id,
                                         Name = budgetCategoryDto.Name,
                                         BudgetCategoryAmountConfigs = budgetCategoryDto.AmountConfigs
                                                                                        .Select(s => new BudgetCategoryAmountConfig
                                                                                                     {
                                                                                                         MonthlyAmount = s.Amount,
                                                                                                         ValidFrom = s.ValidFrom,
                                                                                                         ValidTo = s.ValidTo
                                                                                                     })
                                                                                        .ToList(),
                                         Icon = budgetCategoryDto.Icon ?? "",
                                         Type = budgetCategoryDto.Type
                                     };
                DatabaseContext.Add(categoryEntity);
                DatabaseContext.SaveChanges();
                budgetCategoryDto.BudgetCategoryId = categoryEntity.BudgetCategoryId;
                budgetCategoryDto.Budget = new BudgetDto() {Id = id};
                _ = _budgetsNotifier.CategoryAdded(UserEntity.UserId, budgetCategoryDto);
                return Ok(budgetCategoryDto);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warning, "Exception during category save: " + ex + "; " + (ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
                return BadRequest(new {message = ex.InnerException.Message ?? ex.Message});
            }
        }

        [HttpPost("{id}/categories/{categoryId}")]
        public IActionResult UpdateBudgetCategory(int id, int categoryId, [FromBody] BudgetCategoryDto budgetCategoryDto)
        {
            try
            {
                if (UserEntity.Budgets.All(x => x.BudgetId != id))
                    return BadRequest(new {message = "budgets.notFound"});

                if (budgetCategoryDto.BudgetCategoryId == 0
                    || categoryId == 0
                    || !DatabaseContext.BudgetCategories.Any(x => x.BudgetCategoryId == budgetCategoryDto.BudgetCategoryId))
                {
                    return BadRequest(new {message = "categories.notFound"});
                }

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
                budgetCategoryDto.AmountConfigs
                                 .Where(x => x.ValidFrom == budgetCategoryDto.AmountConfigs.Max(m => m.ValidFrom))
                                 .FirstOrDefault()
                                 .ValidTo = null;


                var categoryEntity = DatabaseContext.BudgetCategories
                                                    .Single(x => x.BudgetCategoryId == budgetCategoryDto.BudgetCategoryId);

                categoryEntity.Name = budgetCategoryDto.Name;
                categoryEntity.Icon = budgetCategoryDto.Icon ?? "";
                DatabaseContext.BudgetCategoryBudgetedAmounts
                               .RemoveRange(categoryEntity.BudgetCategoryBudgetedAmounts);

                categoryEntity.BudgetCategoryBudgetedAmounts = budgetCategoryDto.AmountConfigs
                                                                              .Select(s => new BudgetCategoryBudgetedAmount()
                                                                                           {
                                                                                               MonthlyAmount = s.Amount,
                                                                                               ValidFrom = s.ValidFrom,
                                                                                               ValidTo = s.ValidTo
                                                                                           })
                                                                              .ToList();
                DatabaseContext.SaveChanges();
                budgetCategoryDto.Type = categoryEntity.Type;

                _ = _budgetsNotifier.CategoryUpdated(UserEntity.Id, budgetCategoryDto);

                return Ok(budgetCategoryDto);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warning, "Exception during category save: " + ex + "; " + (ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
                return BadRequest(new {message = ex.InnerException.Message ?? ex.Message});
            }
        }


        [HttpDelete("{budgetId}/categories/{categoryId}")]
        public IActionResult DeleteBudgetCategory(int budgetId, int categoryId)
        {
            try
            {
                if (UserEntity.OwnedBudgets.All(x => x.Id != budgetId))
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

                _ = _budgetsNotifier.CategoryRemoved(UserEntity.Id, categoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        
        */
    }
}