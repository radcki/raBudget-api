using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enum;

namespace raBudget.Core.Interfaces
{
    public interface IResponse
    {
        eResponseType ResponseType { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T Data { get; set; }
    }
}
