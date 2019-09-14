using System;
using raBudget.Domain.Enum;

namespace WebApi.Models.Dtos
{
    public class BaseResult : IEquatable<bool>
    {
        #region Properties

        public eResultType Result { get; set; }
        public string Message { get; set; }

        #endregion

        #region Methods

        public bool Equals(bool other)
        {
            return Result == eResultType.Success;
        }


        public static implicit operator bool(BaseResult me)
        {
            return me.Equals(true);
        }

        #endregion
    }

    public class BaseResult<T> : IEquatable<bool>
    {
        #region Properties

        public T Data { get; set; }
        public eResultType Result { get; set; }
        public string Message { get; set; }

        #endregion

        #region Methods

        public bool Equals(bool other)
        {
            return Result == eResultType.Success;
        }

        public static implicit operator bool(BaseResult<T> me)
        {
            return me.Equals(true);
        }

        public static implicit operator eResultType(BaseResult<T> me)
        {
            return me.Result;
        }

        public static implicit operator T(BaseResult<T> me)
        {
            return me.Data;
        }

        #endregion
    }
}