using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi.Helpers
{
    public static class Extensions
    {
        public static double TransactionsSum(this IEnumerable<BudgetCategory> categories)
        {
            return categories.SelectMany(x => x.Transactions).Sum(x => x.Amount);
        }

        public static double AllocationsSum(this IEnumerable<BudgetCategory> categories)
        {
            return categories.SelectMany(x => x.Allocations).Sum(x => x.Amount);
        }

        public static BudgetCategoryDto ToDto(this BudgetCategory entity)
        {
            return new BudgetCategoryDto
                   {
                       CategoryId = entity.BudgetCategoryId,
                       Type = entity.Type,
                       Name = entity.Name,
                       Amount = entity.MonthlyAmount,
                       Icon = entity.Icon
                   };
        }

        public static BudgetDto ToDto(this Budget entity)
        {
            return new BudgetDto
                   {
                       Name = entity.Name,
                       Id = entity.BudgetId,
                       Currency = entity.Currency,
                       StartingDate = entity.StartingDate,
                       Balance = BalanceHandler.CurrentFunds(entity),
                       Default = entity.BudgetId == entity.User.DefaultBudgetId
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

    }
}