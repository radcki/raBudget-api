namespace WebApi.Models.Dtos
{
    public class PasswordResetDto
    {
        #region Properties

        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }

        #endregion
    }
}