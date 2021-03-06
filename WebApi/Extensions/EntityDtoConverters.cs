﻿namespace WebApi.Extensions
{
    public static class EntityDtoConverters
    {
        /*
        #region Budget category

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
                       TargetBudgetCategoryId = entity.TargetBudgetCategoryId,
                       Type = entity.Type,
                       Name = entity.Name,
                       AmountConfigs = entity.BudgetCategoryAmountConfigs.ToDtoList(),
                       Icon = entity.Icon,
                       Budget = new BudgetDto() {Id = entity.BudgetId, Name = entity.Budget.Name}
                   };
        }

        #endregion

        #region Budget

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

        #endregion

        #region Transaction schedule

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

        #endregion

        #region BudgedCategoryAmountConfig

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
                                            TargetBudgetCategoryId = entity.BudgetCategory.TargetBudgetCategoryId,
                                            Name = entity.BudgetCategory.Name
                                        }
                   };
        }

        #endregion

        public static UserDto ToDto(this User entity)
        {
            return new UserDto()
                      {
                          UserId = entity.UserId,
                          CreationDate = entity.CreationTime,
                          DefaultBudgetId = entity.DefaultBudgetId,
                      };
        }

        public static LogDto ToDto(this Log entity)
        {
            var dto = new LogDto()
                      {
                          Id = entity.Id,
                          Message = entity.Message,
                          Name = entity.Name,
                          Level = (LogLevel) entity.Level,
                          TimeStamp = entity.TimeStamp
                      };
            return dto;
        }
        */
    }
}