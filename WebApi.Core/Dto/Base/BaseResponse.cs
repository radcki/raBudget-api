using raBudget.Core.Interfaces;
using raBudget.Domain.Enum;

namespace raBudget.Core.Dto.Base
{
    public class BaseResponse : IResponse
    {
        #region Properties

        public eResponseType ResponseType { get; set; }

        #endregion
    }

    public class BaseResponse<T> : IResponse<T>
    {
        #region Properties

        public eResponseType ResponseType { get; set; }
        public T Data { get; set; }

        #endregion
    }
}