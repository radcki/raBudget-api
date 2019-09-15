using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using raBudget.Core.Interfaces;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Base
{
    public class BaseResponse : IResponse
    {
        public eResponseType ResponseType { get; set; }
    }

    public class BaseResponse<T> : IResponse<T>
    {
        public eResponseType ResponseType { get; set; }
        public T Data { get; set; }
    }

}
