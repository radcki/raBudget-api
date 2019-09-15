namespace raBudget.Domain.Enum
{
    public enum eResponseType
    {
        Error = 0,
        Success = 1,
        Unauthorized = 2,
        NoDataFound = 3,
    }

    public static class eResponseTypeExtensions
    {
        public static bool IsSuccess(this eResponseType result)
        {
            return result == eResponseType.Success;
        }
    }
}