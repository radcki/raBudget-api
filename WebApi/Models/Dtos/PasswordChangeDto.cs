namespace WebApi.Models.Dtos
{
    public class PasswordChangeDto
    {
        #region Properties

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        #endregion
    }
}