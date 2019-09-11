using System;
using AutoMapper.Mappers;
using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
{
    public class BaseResult : IEquatable<bool>
    {
        public eResultType Result { get; set; }
        public string Message { get; set; }

        public bool Equals(bool other)
        {
            return Result == eResultType.Success;
        }


        public static implicit operator bool(BaseResult me)
        {
            return me.Equals(true);
        }
    }

    public class BaseResult<T> : IEquatable<bool>
    {
        public T Data { get; set; }
        public eResultType Result { get; set; }
        public string Message { get; set; }
        
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
    }
}
