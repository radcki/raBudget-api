using System;
using System.Collections.Generic;

namespace raBudget.Domain.ExtensionMethods
{
    public static class GenericExtensionMethods
    {
        #region Methods

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

        public static bool IsNullOrDefault<T>(this T val)
        {
            return val == null || EqualityComparer<T>.Default.Equals(val, default) || EqualityComparer<T>.Default.Equals(val);
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

        #endregion
    }
}