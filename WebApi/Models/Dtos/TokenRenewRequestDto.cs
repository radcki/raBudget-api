namespace WebApi.Models.Dtos
{
    public class TokenRenewRequestDto
    {
        #region Properties

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }

        #endregion
    }
}