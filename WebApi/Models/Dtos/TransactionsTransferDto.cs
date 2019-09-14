namespace WebApi.Models.Dtos
{
    public class TransactionsTransferDto
    {
        #region Properties

        public int BudgetId { get; set; }
        public int SourceCategory { get; set; }
        public int TargetCategory { get; set; }

        #endregion
    }
}