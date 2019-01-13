using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;
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
                       AmountConfigs = entity.BudgetCategoryAmountConfigs
                                               .Select(x => new BudgetCategoryAmountConfigDto() {
                                                   Amount = x.MonthlyAmount,
                                                   ValidFrom = x.ValidFrom,
                                                   ValidTo = x.ValidTo
                                               })
                                               .ToList(),
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
                       Default = entity.BudgetId == entity.User.DefaultBudgetId,
                       IncomeCategories = entity.BudgetCategories.Where(x=>x.Type == eBudgetCategoryType.Income).Select(x=>x.ToDto()).ToList(),
                       SpendingCategories = entity.BudgetCategories.Where(x=>x.Type == eBudgetCategoryType.Spending).Select(x=>x.ToDto()).ToList(),
                       SavingCategories = entity.BudgetCategories.Where(x=>x.Type == eBudgetCategoryType.Saving).Select(x=>x.ToDto()).ToList(),
                   };
        }

        public static BudgetCategoryAmountConfigDto ToDto (this BudgetCategoryAmountConfig entity)
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
                          Level = (LogLevel) entity.Level,
                          TimeStamp = entity.TimeStamp
                      };
            return dto;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

    }
}