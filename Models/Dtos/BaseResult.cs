using WebApi.Models.Enum;

namespace WebApi.Models.Dtos
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
