using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Core.Interfaces;

namespace raBudget.Core.Dto.Common
{
    public abstract class BaseResponse<T> : IResponse<T>
    {
        public T Data { get; set; }
    }

    public abstract class CollectionResponse<T> : BaseResponse<IEnumerable<T>>
    {
    }
}
