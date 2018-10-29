using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Enum;

namespace WebApi.Dtos
{
    public class BaseResult
    {
        public eResultType Result { get; set; }
        public string Message { get; set; }
    }

    public class BaseResult<T>
    {
        public T Data { get; set; }
        public eResultType Result { get; set; }
        public string Message { get; set; }
    }
}
