using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;

namespace WebApi.Extensions
{
    public static class ExtensionMethods
    {
        public static Guid? UserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claimId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(claimId, out var userId);
            if (userId.IsNullOrDefault())
            {
                return null;
            }

            return userId;
        }
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