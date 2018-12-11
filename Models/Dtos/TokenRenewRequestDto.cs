namespace WebApi.Models.Dtos
{
    public class TokenRenewRequestDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
    }
}