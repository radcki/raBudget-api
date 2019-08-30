using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi.Extensions
{
    public static class EntityDtoConverters
    {
        public static List<BudgetCategoryDto> ToDtoList(this IEnumerable<BudgetCategory> entities)
        {
            return entities.ToDtoEnumerable().ToList();
        }
        public static IEnumerable<BudgetCategoryDto> ToDtoEnumerable(this IEnumerable<BudgetCategory> entities)
        {
            foreach (var entity in entities)
            {
                yield return entity.ToDto();
            }
        }

        public static BudgetCategoryDto ToDto(this BudgetCategory entity)
        {
            return new BudgetCategoryDto
            {
                CategoryId = entity.BudgetCategoryId,
                Type = entity.Type,
                Name = entity.Name,
                AmountConfigs = entity.BudgetCategoryAmountConfigs
                                             .Select(x => new BudgetCategoryAmountConfigDto()
                                             {
                                                 Amount = x.MonthlyAmount,
                                                 ValidFrom = x.ValidFrom,
                                                 ValidTo = x.ValidTo
                                             })
                                             .ToList(),
                Icon = entity.Icon,
                Budget = new BudgetDto() { Id = entity.BudgetId, Name = entity.Budget.Name }
            };
        }

        public static List<BudgetDto> ToDtoList(this IEnumerable<Budget> entities)
        {
            return entities.ToDtoEnumerable().ToList();
        }
        public static IEnumerable<BudgetDto> ToDtoEnumerable(this IEnumerable<Budget> entities)
        {
            foreach (var entity in entities)
            {
                yield return entity.ToDto();
            }
        }

        public static BudgetDto ToDto(this Budget entity)
        {
            var categories = entity.BudgetCategories.ToDtoEnumerable();
            var budget = new BudgetDto
            {
                Name = entity.Name,
                Id = entity.BudgetId,
                Currency = entity.Currency,
                StartingDate = entity.StartingDate,

                Default = entity.BudgetId == entity.User.DefaultBudgetId,
                IncomeCategories = categories.Where(x => x.Type == eBudgetCategoryType.Income).ToList(),
                SpendingCategories = categories.Where(x => x.Type == eBudgetCategoryType.Spending).ToList(),
                SavingCategories = categories.Where(x => x.Type == eBudgetCategoryType.Saving).ToList(),
            };

            budget.Balance = BalanceHandler.CurrentFunds(entity);

            return budget;
        }

        public static List<TransactionScheduleDto> ToDtoList(this IEnumerable<TransactionSchedule> entities)
        {
            return entities.ToDtoEnumerable().ToList();
        }
        public static IEnumerable<TransactionScheduleDto> ToDtoEnumerable(this IEnumerable<TransactionSchedule> entities)
        {
            foreach (var entity in entities)
            {
                yield return entity.ToDto();
            }
        }
        public static TransactionScheduleDto ToDto(this TransactionSchedule entity)
        {
            var dto = new TransactionScheduleDto()
            {
                TransactionScheduleId = entity.TransactionScheduleId,
                Amount = entity.Amount,
                BudgetCategory = entity.BudgetCategory.ToDto(),
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Frequency = entity.Frequency,
                PeriodStep = entity.PeriodStep,
                // Lista transakcji zostaje tutaj pominięta jako zazwyczaj niepotrzebna
            };
            return dto;
        }

        public static List<BudgetCategoryAmountConfigDto> ToDtoList(this IEnumerable<BudgetCategoryAmountConfig> entities)
        {
            return entities.ToDtoEnumerable().ToList();
        }
        public static IEnumerable<BudgetCategoryAmountConfigDto> ToDtoEnumerable(this IEnumerable<BudgetCategoryAmountConfig> entities)
        {
            foreach (var entity in entities)
            {
                yield return entity.ToDto();
            }
        }
        public static BudgetCategoryAmountConfigDto ToDto(this BudgetCategoryAmountConfig entity)
        {
            return new BudgetCategoryAmountConfigDto
            {
                Amount = entity.MonthlyAmount,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo,
                BudgetCategory = new BudgetCategoryDto
                {
                    CategoryId = entity.BudgetCategory.BudgetCategoryId,
                    Name = entity.BudgetCategory.Name
                }
            };
        }

        public static LogDto ToDto(this Log entity)
        {
            var dto = new LogDto()
            {
                Id = entity.Id,
                Message = entity.Message,
                Name = entity.Name,
                Level = (LogLevel)entity.Level,
                TimeStamp = entity.TimeStamp
            };
            return dto;
        }
    }
}
