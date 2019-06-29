using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            double sum = 0;
            if (categories == null)
            {
                return sum;
            }

            foreach (var budgetCategory in categories)
            {
                if (budgetCategory.Transactions == null) { continue;}
                sum += budgetCategory.TransactionsSum ?? budgetCategory.Transactions.Sum(x => x.Amount);
            }

            return sum;
        }

        public static double AllocationsSum(this IEnumerable<BudgetCategory> categories)
        {
            double sum = 0;
            if (categories == null)
            {
                return sum;
            }

            foreach (var budgetCategory in categories)
            {
                if (budgetCategory.Transactions == null) { continue; }

                sum += budgetCategory.AllocationsSum ?? budgetCategory.Allocations.Sum(x => x.Amount);
            }

            return sum;
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
                       Budget = new BudgetDto() {Id = entity.BudgetId, Name = entity.Budget.Name}
                   };
        }

        public static BudgetDto ToDto(this Budget entity)
        {
            var categories = entity.BudgetCategories.Select(x => x.ToDto()).ToList();
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
                          Level = (LogLevel) entity.Level,
                          TimeStamp = entity.TimeStamp
                      };
            return dto;
        }

        public static List<DateTime> OccurrencesInPeriod(this TransactionScheduleDto schedule, DateTime from, DateTime to)
        {
            var entity = new TransactionSchedule()
                         {
                             StartDate = schedule.StartDate,
                             EndDate = schedule.EndDate,
                             PeriodStep = schedule.PeriodStep,
                             Frequency = schedule.Frequency
                         };
            return entity.OccurrencesInPeriod(from, to);
        }

        public static List<DateTime> OccurrencesInPeriod(this TransactionSchedule schedule, DateTime from, DateTime to)
        {
            var start = new[] {schedule.StartDate, from}.Max();
            var end = schedule.EndDate.IsNullOrDefault() ? to : new[] {schedule.EndDate.Value, to}.Min();

            var allOccurrences = new List<DateTime>();
            var current = new DateTime(schedule.StartDate.Ticks);
            bool exitLoop = false;

            while (current <= end)
            {
                allOccurrences.Add(current);
                switch (schedule.Frequency)
                {
                    case eFrequency.Monthly:
                        current = current.AddMonths(schedule.PeriodStep);
                        break;
                    case eFrequency.Weekly:
                        current = current.AddDays(7 * schedule.PeriodStep);
                        break;
                    case eFrequency.Daily:
                        current = current.AddDays(schedule.PeriodStep);
                        break;
                    default:
                    case eFrequency.Once: //już dodany przed switch
                        exitLoop = true;
                        break;
                }

                if (exitLoop)
                {
                    break;
                }
            }

            return allOccurrences.Where(x => x >= start).Distinct().ToList();
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
                    res.Append(valid[(int) (num % (uint) valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static bool IsNullOrDefault<T>(this T val)
        {
            return val == null || EqualityComparer<T>.Default.Equals(val, default(T));
        }

        public static T Max<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) > 0)
                return first;
            return second;
        }

        public static T Min<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) < 0)
                return first;
            return second;
        }
    }
}