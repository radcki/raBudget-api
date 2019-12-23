using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Core.Interfaces
{
    public interface IResponse<T>
    {
        T Data { get; set; }
    }
}
