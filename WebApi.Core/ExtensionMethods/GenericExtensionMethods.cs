using System.Collections.Generic;
using raBudget.Domain.Entities;

namespace raBudget.Core.ExtensionMethods
{
    public static class GenericExtensionMethods
    {
        #region Methods

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