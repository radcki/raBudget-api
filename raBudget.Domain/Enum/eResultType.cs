namespace raBudget.Domain.Enum
{
    public enum eResultType
    {
        Error = 0,
        Success = 1,
        Unauthorized = 2,
        NotFound = 3
    }

    public static class eResultTypeExtensions
    {
        public static bool IsSuccess(this eResultType result)
        {
            return result == eResultType.Success;
        }
    }
}